using System.Globalization;
using System.Text.Json.Serialization;
using Application.Common;
using Application.Features.Products.Commands.CreateProduct;
using Application.Features.Suppliers.Commands.CreateSupplier;
using Application.Jobs;
using Application.Mappings;
using Application.Validation;
using Domain.Interfaces;
using FirebaseAdmin;
using FluentValidation;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.PostgreSql;
using HealthChecks.UI.Client;
using Infrastructure;
using Infrastructure.HealthChecks;
using Infrastructure.Repositories;
using Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Minio;
using Presentation.Errors;
using Presentation.Filters;
using Presentation.Middleware;
using Serilog;
using StackExchange.Redis;
using AuthorizationMiddleware = Presentation.Middleware.AuthorizationMiddleware;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// now every endpoint logs its actions
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ActionLoggingFilter>();
    options.Filters.AddService<ModelValidationFilter>();
})
.AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<CultureQueryParameterFilter>();
    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter ID token",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document),
            Array.Empty<string>().ToList()
        }
    });
});

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        // in case of a bad request
        var response = new ErrorResponse(
            ErrorCodes.Validation,
            "Invalid request data.",
            context.HttpContext.TraceIdentifier);
        
        return new BadRequestObjectResult(response);
    };
});

var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("fr"),
    new CultureInfo("ar")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = 
        builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "WarehouseManagementApi";
});

builder.Services.AddDbContext<WarehouseDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        // to tell migrations to save in infrastructure project
        b => b.MigrationsAssembly("Infrastructure")
    ));

builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "postgresql-check",
        tags: ["db"])
    .AddCheck<RedisRetryHealthCheck>("Redis", tags: ["cache"]);

builder.Services.AddHealthChecksUI(setup =>
{
    setup.AddHealthCheckEndpoint(
        "Warehouse API",
        "/health");
    
    setup.SetEvaluationTimeInSeconds(15);
    setup.MaximumHistoryEntriesPerEndpoint(50);
}).AddInMemoryStorage();

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
    builder.Configuration["FirebaseServiceAccountPath"]);

FirebaseApp.Create();

var firebaseProjectId = builder.Configuration["Firebase:ProjectId"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.Authority = $"https://securetoken.google.com/{firebaseProjectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{firebaseProjectId}",
            ValidateAudience = true,
            ValidAudience = firebaseProjectId,
            ValidateLifetime = true,
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"))
    .AddPolicy("AuthenticatedUser", policy =>
        policy.RequireAuthenticatedUser());

builder.Services.AddHttpClient("FirebaseAuth", client =>
{
    client.BaseAddress = new Uri("https://identitytoolkit.googleapis.com/v1/");
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IWarehouseItemRepository, WarehouseItemRepository>();
builder.Services.AddScoped<ActionLoggingFilter>();
builder.Services.AddScoped<ModelValidationFilter>();
builder.Services.AddScoped<IExpiryCheckJob, ExpiryCheckJob>();
builder.Services.AddSingleton<ICacheStatsTracker, CacheStatsTracker>();
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));

builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationMiddleware>();

builder.Services.Configure<MinIoStorage>(builder.Configuration.GetSection("MinIO"));
builder.Services.AddSingleton<IMinioClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<MinIoStorage>>().Value;
    return new MinioClient()
        .WithEndpoint(options.Endpoint)
        .WithCredentials(options.AccessKey, options.SecretKey)
        .WithSSL(options.UseSsl)
        .Build();
});

builder.Services.AddScoped<IFileStorageService, MinIoStorageService>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateSupplierCommand).Assembly);
    cfg.AddOpenBehavior(typeof(FluentValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(CreateProductCommand).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(CreateSupplierCommand).Assembly);
builder.Services.AddAutoMapper(cfg => {}, typeof(ProductMapper).Assembly);
builder.Services.AddHangfire(config => config
    .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(
        builder.Configuration.GetConnectionString("DefaultConnection")),
        new PostgreSqlStorageOptions
        {
            PrepareSchemaIfNecessary = true
        }));
builder.Services.AddHangfireServer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestLocalization();
app.UseSerilogRequestLogging();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SlowRequestLoggingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
    options.ApiPath = "/health-ui-api";
});
app.MapControllers();

app.UseHangfireDashboard();
RecurringJob.AddOrUpdate<IExpiryCheckJob>(
    "check-expiring-products",
    job => job.CheckExpiringProductsAsync(CancellationToken.None),
    Cron.Daily);

app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

app.Run();

// makes sure no logs are lost before shutdown
Log.CloseAndFlush();