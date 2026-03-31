using Expo.Application.Interfaces.Services;

namespace Expo.API.Services;

internal class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string? UserName =>
        _httpContextAccessor.HttpContext?.User?.Identity?.Name;
}
