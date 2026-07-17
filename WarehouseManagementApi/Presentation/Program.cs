using System.Text.Json.Serialization;
using Application.Features.Products.Commands.CreateProduct;
using Application.Features.Suppliers.Commands.CreateSupplier;
using Application.Mappings;
using Application.Validation;
using Domain.Interfaces;
using FluentValidation;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Errors;
using Presentation.Filters;
using Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

// now every endpoint logs its actions
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ActionLoggingFilter>();
    options.Filters.AddService<ModelValidationFilter>();
})
.AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddDbContext<WarehouseDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        // to tell migrations to save in infrastructure project
        b => b.MigrationsAssembly("Infrastructure")
    ));

builder.Services.AddValidatorsFromAssembly(typeof(CreateProductCommand).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(CreateSupplierCommand).Assembly);
builder.Services.AddAutoMapper(cfg => {}, typeof(ProductMapper).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// every action needs a correlation ID, that's why it's first
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestTimingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();


app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

app.Run();