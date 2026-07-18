using System.Globalization;
using System.Text.Json.Serialization;
using Application.Features.Products.Commands.CreateProduct;
using Application.Features.Suppliers.Commands.CreateSupplier;
using Application.Mappings;
using Application.Validation;
using Domain.Interfaces;
using FluentValidation;
using HealthChecks.UI.Client;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Errors;
using Presentation.Filters;
using Presentation.Middleware;
using Serilog;
using StackExchange.Redis;

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
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!,
        name: "redis-check",
        tags: ["cache"]);

builder.Services.AddHealthChecksUI(setup =>
{
    setup.AddHealthCheckEndpoint(
        "Warehouse API",
        "/health");
    
    setup.SetEvaluationTimeInSeconds(15);
    setup.MaximumHistoryEntriesPerEndpoint(50);
}).AddInMemoryStorage();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ActionLoggingFilter>();
builder.Services.AddScoped<ModelValidationFilter>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateSupplierCommand).Assembly);
    cfg.AddOpenBehavior(typeof(FluentValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(CreateProductCommand).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(CreateSupplierCommand).Assembly);
builder.Services.AddAutoMapper(cfg => {}, typeof(ProductMapper).Assembly);

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
app.UseMiddleware<RequestTimingMiddleware>();
app.UseHttpsRedirection();
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


app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

app.Run();

// makes sure no logs are lost before shutdown
Log.CloseAndFlush();