using FluentResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Expo.API.Controllers
{
    /// <summary>
    /// Base controller with shared helpers for logging, image handling, and result mapping.
    /// </summary>
    [ApiController]
    public abstract class BaseController(ILogger logger) : ControllerBase
    {
        protected readonly ILogger _logger = logger;

        /// <summary>
        /// Map a Result&lt;T&gt; from the service to IActionResult
        /// </summary>
        protected IActionResult ToActionResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                if (result.Value == null)
                    return Ok(); // per success senza payload
                return Ok(result.Value);
            }
            else
            {
                // Se vuoi distinguere errori not found da bad request puoi fare un check sui messaggi
                // ad esempio se contiene "not found" -> return NotFound
                var firstError = result.Errors.FirstOrDefault()?.Message ?? "Unknown error";
                if (firstError.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(firstError);
                return BadRequest(firstError);
            }
        }

        /// <summary>
        /// Handle image upload logic consistently across controllers
        /// </summary>
        protected async Task<IActionResult> HandleImageUpload(
            Func<Task<Result<string>>> uploadFunc,
            string entityName,
            int entityId)
        {
            try
            {
                var result = await uploadFunc();
                if (result.IsSuccess)
                    return Ok(result.Value);

                if (result.Errors.Any(e => e.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)))
                    return NotFound(result.Errors.First().Message);

                return BadRequest(result.Errors.First().Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image for {Entity} {Id}", entityName, entityId);
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Handle image deletion logic consistently across controllers
        /// </summary>
        protected async Task<IActionResult> HandleImageDelete(
            Func<Task<Result<bool>>> deleteFunc,
            string entityName,
            int entityId)
        {
            try
            {
                var result = await deleteFunc();
                if (result.IsSuccess)
                    return Ok();

                if (result.Errors.Any(e => e.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)))
                    return NotFound(result.Errors.First().Message);

                return BadRequest(result.Errors.First().Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image for {Entity} {Id}", entityName, entityId);
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Wrap async service call with exception logging
        /// </summary>
        protected async Task<IActionResult> HandleServiceCall<T>(
            Func<Task<Result<T>>> serviceCall,
            string operationDescription)
        {
            try
            {
                var result = await serviceCall();
                return ToActionResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during {Operation}", operationDescription);
                return Problem(ex.Message);
            }
        }
    }
}