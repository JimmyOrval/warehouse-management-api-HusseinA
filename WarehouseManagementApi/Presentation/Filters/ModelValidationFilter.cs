using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.Filters;

public class ModelValidationFilter : IActionFilter
{
    /*
     * by using [ApiController], there is no need for us to use
     * validation filters since it already does them automatically
     * but I used it just to learn its uses (creating a product/supplier)
     */
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if(!context.ModelState.IsValid)
            context.Result = new BadRequestObjectResult(context.ModelState);
    }

    public void OnActionExecuted(ActionExecutedContext context) {}
}