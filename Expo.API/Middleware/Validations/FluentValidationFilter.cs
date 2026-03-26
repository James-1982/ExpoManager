using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Expo.API.Middleware.Validations;

public class FluentValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Controller is not Controller)
        {
            if (!context.ModelState.IsValid)
            {
                var firstError = context.ModelState
                   .Where(ms => ms.Value?.Errors.Count > 0)
                   .SelectMany(ms => ms.Value.Errors)
                   .FirstOrDefault();

                context.Result = new JsonResult(firstError?.ErrorMessage) { StatusCode = 400 };
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
