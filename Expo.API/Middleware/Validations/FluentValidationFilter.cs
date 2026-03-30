using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Expo.API.Middleware.Validations;

public class FluentValidationFilter : IActionFilter
{
    private readonly ILogger<FluentValidationFilter> _logger;

    public FluentValidationFilter(ILogger<FluentValidationFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(ms => ms.Value?.Errors.Count > 0)
                .ToDictionary(
                    ms => ms.Key,
                    ms => ms.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            _logger.LogWarning("Validation failed for action {Action}: {@Errors}", 
                context.ActionDescriptor.DisplayName, errors);

            context.Result = new JsonResult(new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                title = "One or more validation errors occurred.",
                status = 400,
                errors = errors
            })
            {
                StatusCode = 400
            };
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}