using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.Filters;

public class ActionLoggingFilter(ILogger<ActionLoggingFilter> logger) : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        logger.LogInformation("Executing action {ActionName}",
            context.ActionDescriptor.DisplayName);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        logger.LogInformation("Executed action {ActionName}",
            context.ActionDescriptor.DisplayName);
    }
}