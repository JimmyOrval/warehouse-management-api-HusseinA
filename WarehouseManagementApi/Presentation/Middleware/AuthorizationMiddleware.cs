using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.Localization;
using Presentation.Errors;
using Presentation.Resources;

namespace Presentation.Middleware;

/*
 this class differs from the exception handler
 since auth uses its own Authorization middleware
 and doesn't actually throw anything, so I need
 this class to format the responses properly
*/
public class AuthorizationMiddleware(
    IStringLocalizer<ErrorMessages> localizer,
    ILogger<AuthorizationMiddleware> logger)
    : IAuthorizationMiddlewareResultHandler
{
    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if(authorizeResult.Succeeded)
        {
            await next(context);
            return;
        }
        
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        var (statusCode, errorCode, resourceKey) = isAuthenticated
            ? (StatusCodes.Status403Forbidden, ErrorCodes.Forbidden, "Forbidden")
            : (StatusCodes.Status401Unauthorized, ErrorCodes.Unauthorized, "Unauthorized");

        logger.LogWarning(errorCode == ErrorCodes.Forbidden
            ? $"{errorCode}: You do not have permission to perform this action"
            : $"{errorCode}: Authentication is required");

        var message = ErrorMessages.ResourceManager.GetString(resourceKey, CultureInfo.CurrentCulture);
        
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var response = new ErrorResponse(errorCode, message!, context.TraceIdentifier);
        await context.Response.WriteAsJsonAsync(response);
    }
}