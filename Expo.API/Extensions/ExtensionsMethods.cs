using Expo.Domain.Interfaces.Repositories;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Expo.API.Extensions;

/// <summary>
/// Extension methods
/// </summary>
public static class ExtensionsMethods
{
    /// <summary>
    /// Return base URL =>  "https://example.com[:porta]"
    /// </summary>
    /// <param name="controller"></param>
    /// <returns></returns>
    public static string GetBaseUrl(this ControllerBase controller)
    {
        return $"{controller.Request.Scheme}://{controller.Request.Host}";
    }

    /// <summary>
    /// Get API Main version => "1" 
    /// </summary>
    /// <param name="controller"></param>
    /// <returns></returns>
    public static string GetApiVersionNumber(this ControllerBase controller)
    {
        var requestedVersion = controller.HttpContext.GetRequestedApiVersion()?.ToString() ?? "1";

        return requestedVersion.Split('.')[0];
    }

    /// <summary>
    /// Return base URL with API version => "https://example.com[:porta]/api/v{mainversion}"
    /// </summary>
    /// <param name="controller"></param>
    /// <returns></returns>
    public static string GetApiVersionBaseUrl(this ControllerBase controller)
    {
        return $"{controller.GetBaseUrl()}/api/v{controller.GetApiVersionNumber()}";
    }
}
