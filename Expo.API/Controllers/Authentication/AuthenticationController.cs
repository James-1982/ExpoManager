using Asp.Versioning;
using Expo.API.Extensions;
using Expo.API.Utils;
using Expo.Domain.Constants;
using Expo.Domain.DTO.DB;
using Expo.Domain.DTO.User;
using Expo.Domain.Interfaces.Services;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;

namespace Expo.API.Controllers.Authentication;

internal static class AuthEndpoints
{
    internal const string ConfirmRegistration = "confirm-email";
    internal const string ResetPassword = "reset-password";
    internal const string ForgotPassword = "forgot-password";
    internal const string Register = "register";
    internal const string Login = "login";
    internal const string Logout = "logout";
    internal const string Refresh = "refresh";
}

/// <summary>
/// Authentication controller
/// </summary>
/// <param name="logger">Logger</param>
/// <param name="mapper">Class to map object</param>
/// <param name="authService">Authentication Service</param>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(ApiConstants.V1)]
public class AuthenticationController(
    ILogger<AuthenticationController> logger,
    IMapper mapper,
    IAuthenticationService authService) : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly IAuthenticationService _authService = authService;

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="dto">User input data</param>
    /// <returns></returns>
    [HttpPost(AuthEndpoints.Register)]
    [MapToApiVersion(ApiConstants.V1)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        var model = _mapper.From(dto)
          .AddParameters("BaseUrl", this.GetBaseUrl())
          .AdaptToType<RegisterUserDto>();

        model.Role = RoleHierarchy.GetRoleName(Role.Supervisor);

        // Passa base URL senza email e token
        var baseUrl = $"{this.GetApiVersionBaseUrl()}/{this.ControllerContext.ActionDescriptor.ControllerName.ToLower()}/{AuthEndpoints.ConfirmRegistration}";
        var result = await _authService.RegisterAsync(model, baseUrl);

        return result.IsSuccess
            ? Ok("Registration successful! Check your email to confirm.")
            : BadRequest("Registration failed");
    }

    /// <summary>
    /// Confirmation endpoit for regostered user
    /// </summary>
    /// <param name="userId">user id</param>
    /// <param name="token">token</param>
    /// <returns></returns>
    [HttpGet(AuthEndpoints.ConfirmRegistration)]
    [MapToApiVersion(ApiConstants.V1)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        var result = await _authService.ConfirmEmailAsync(userId, token);

        return result.IsSuccess
            ? Ok("Email confirmed successfully")
            : BadRequest("Invalid token or userId");
    }

    /// <summary>
    /// Execute a login
    /// </summary>
    /// <param name="model">login data</param>
    /// <returns></returns>
    [HttpPost(AuthEndpoints.Login)]
    [MapToApiVersion(ApiConstants.V1)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
    {
        _logger.LogInformation($"Attempting login for user: {model.Email}");

        var tokenResult = await _authService.LoginAsync(model);

        if (tokenResult.IsFailed)
        {
            _logger.LogWarning(tokenResult.Errors[0].Message);
            return Unauthorized(new { message = $"Invalid credentials for user {model.Email}" });
        }

        _logger.LogInformation($"User logged in successfully: {model.Email}");

        return Ok(new
        {
            token = tokenResult.Value.Value.Token,
            refresh = tokenResult.Value.Value.RefreshToken,
            expiration = tokenResult.Value.Value.Expiration
        });
    }

    /// <summary>
    /// Execute a logout
    /// </summary>
    [HttpPost(AuthEndpoints.Logout)]
    [MapToApiVersion(ApiConstants.V1)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return BadRequest("User not found in token.");

        var result = await _authService.LogoutAsync(userId);

        return result.IsSuccess
            ? Ok(new { success = true, message = "Logged out successfully." })
            : BadRequest("Fail to logout");
    }

    /// <summary>
    /// Refresh the token
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost(AuthEndpoints.Refresh)]
    [MapToApiVersion(ApiConstants.V1)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken);

        return result.IsSuccess
            ? Ok(result)
            : Unauthorized();
    }
    /// <summary>
    ///  Request password reset
    /// </summary>
    /// <param name="model">user email</param>
    /// <returns></returns>
    [HttpPost(AuthEndpoints.ForgotPassword)]
    [MapToApiVersion(ApiConstants.V1)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] EmailDto model)
    {
        _logger.LogInformation($"Password reset requested for user: {model.Email}");

        // Passa base URL senza email e token
        var baseUrl = $"{this.GetBaseUrl()}/reset-password.html?version={this.GetApiVersionNumber()}";

        var result = await _authService.ForgotPasswordAsync(model.Email, baseUrl);

        if (result.IsFailed)
        {
            _logger.LogWarning($"Password reset failed");
            return Ok();
        }

        _logger.LogInformation($"Password reset link sent to: {model.Email}");
        return Ok("Check your email to reset password.");
    }

    /// <summary>
    /// Reset password
    /// </summary>
    /// <param name="model">reset password data: email, token and new password</param>
    /// <returns></returns>
    [HttpPost(AuthEndpoints.ResetPassword)]
    [MapToApiVersion(ApiConstants.V1)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto model)
    {
        _logger.LogInformation($"Resetting password for user: {model.Email}");

        // Decodifica il token da Base64Url
        var tokenBytes = WebEncoders.Base64UrlDecode(model.Token);
        var decodedToken = System.Text.Encoding.UTF8.GetString(tokenBytes);

        var result = await _authService.ResetPasswordAsync(model.Email, decodedToken, model.NewPassword);

        if (result.IsFailed)
        {
            _logger.LogWarning($"Password reset failed for user: {model.Email}");
            return BadRequest(new { message = $"Password reset failed for user {model.Email}" });
        }

        _logger.LogInformation($"Password reset successful for user: {model.Email}");
        return Ok(new { message = $"Password reset successful for user {model.Email}" });
    }
}