using System.Text.Json;
using Domain.Exceptions;
using Presentation.Errors;

namespace Presentation.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path);
            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context, Exception exception)
    {
        var (statusCode, error, message) = exception switch
        {
            NotFoundException =>
                (StatusCodes.Status404NotFound,
                    ErrorCodes.NotFound,
                    exception.Message),

            BusinessRuleException =>
                (StatusCodes.Status409Conflict,
                    ErrorCodes.BusinessRule,
                    exception.Message),
            
            RequestValidationException =>
                (StatusCodes.Status400BadRequest,
                    ErrorCodes.Validation,
                    exception.Message),
            
            _ => (StatusCodes.Status500InternalServerError,
                    ErrorCodes.Internal,
                    exception.Message)
        };
        
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        
        var response = new ErrorResponse(
            error, message, context.TraceIdentifier);
        
        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}