using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Expo.API.Middleware;

/// <summary>
/// Middleware to centralize API exeptions
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="next"></param>
/// <param name="logger"></param>
public class ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger = logger;
    /// <summary>
    /// Invoke method
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problem.Status.Value;

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
