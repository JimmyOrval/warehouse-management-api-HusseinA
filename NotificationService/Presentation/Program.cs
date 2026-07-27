using System.Text.Json.Serialization;
using Application.Common;
using Application.Features.Notifications.Commands.CreateNotificationFromEvent;
using Application.Features.Notifications.Queries.ListNotifications;
using Application.Mappers;
using Application.Validation;
using Domain.Interfaces;
using Microsoft.OpenApi;
using Presentation.Filters;
using FirebaseAdmin;
using FluentValidation;
using Infrastructure;
using Infrastructure.Messaging;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Presentation.Errors;
using Presentation.Middleware;
using Serilog;
using AuthorizationMiddleware = Presentation.Middleware.AuthorizationMiddleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ActionLoggingFilter>();
})
.AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter()));

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

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var response = new ErrorResponse(
            ErrorCodes.Validation,
            "Invalid request data.",
            context.HttpContext.TraceIdentifier);

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Infrastructure")));

var serviceAccountPath = builder.Configuration["FirebaseServiceAccountPath"];
if(!string.IsNullOrEmpty(serviceAccountPath))
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", serviceAccountPath);

if (FirebaseApp.DefaultInstance == null && !string.IsNullOrEmpty(serviceAccountPath))
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
    .AddPolicy("AuthenticatedUser", policy =>
        policy.RequireAuthenticatedUser());

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ActionLoggingFilter>();
builder.Services.AddScoped<INotificationConsumptionService, NotificationConsumptionService>();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationMiddleware>();

builder.Services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddHostedService<RabbitMqConsumer>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(CreateNotificationFromEventCommand)
            .Assembly);
    cfg.RegisterServicesFromAssembly(typeof(NotificationDbContext).Assembly);
    cfg.AddOpenBehavior(typeof(FluentValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(CreateNotificationFromEventCommand).Assembly);
builder.Services.AddAutoMapper(cfg => { },
    typeof(NotificationMapper).Assembly);
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(ListNotificationsQuery).Assembly);
    cfg.AddOpenBehavior(typeof(FluentValidationBehavior<,>));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SlowRequestLoggingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

app.Run();

Log.CloseAndFlush();