using System.Globalization;
using System.Text.Json;
using Domain.Exceptions;
using Presentation.Errors;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Presentation.Resources;

namespace Presentation.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IStringLocalizer<ErrorMessages> localizer)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception,
                "Unhandled exception for {Method} {Path} TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            if (context.RequestServices.GetRequiredService<IHostEnvironment>().IsEnvironment("Testing"))
            {
                Console.WriteLine($"[DIAGNOSTIC] {exception}");
            }

            await HandleExceptionAsync(context, exception, localizer);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context, Exception exception, IStringLocalizer<ErrorMessages> localizer)
    {
        var (statusCode, error, resourceKey) = exception switch
        {
            NotFoundException =>
                (StatusCodes.Status404NotFound,
                    ErrorCodes.NotFound,
                    "NotFound"),

            BusinessRuleException =>
                (StatusCodes.Status409Conflict,
                    ErrorCodes.BusinessRule,
                    "BusinessRule"),
            
            ValidationException =>
                (StatusCodes.Status400BadRequest,
                    ErrorCodes.Validation,
                    "Validation"),
            
            StorageException =>
                (StatusCodes.Status502BadGateway,
                    ErrorCodes.Storage,
                    "Storage"),
            
            _ => (StatusCodes.Status500InternalServerError,
                    ErrorCodes.Internal,
                    "Internal")
        };
        
        var localizedMessage = ErrorMessages.ResourceManager.GetString(
            resourceKey,
            CultureInfo.CurrentUICulture);
        
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        
        var response = new ErrorResponse(
            error, localizedMessage, context.TraceIdentifier);
        
        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}