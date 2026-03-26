using Expo.Domain.DTO.User;
using FluentResults;

namespace Expo.Domain.Interfaces.Services;

/// <summary>
/// Service to Authenticate users
/// </summary>
/// <remarks>
/// Contract that define methods to manage authentication's operations.
/// </remarks>
public interface IAuthenticationService
{
    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="model">Input data for register a new user</param>
    /// <param name="confirmationLinkBaseUrl">Base part for confirmation link</param>
    /// <returns>A task  with bool result of operation</returns>
    Task<Result<bool>> RegisterAsync(RegisterUserDto model, string confirmationLinkBaseUrl);
    /// <summary>
    /// Confirm email registration
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="token">Confirmation token</param>
    /// <returns>A task  with bool result of operation</returns>
    Task<Result<bool>> ConfirmEmailAsync(string userId, string token);
    /// <summary>
    /// Execute login and return expiring token 
    /// </summary>
    /// <param name="model">Login input data</param>
    /// <returns>A task  with tokens after login</returns>
    Task<Result<(string Token, string RefreshToken, DateTime Expiration)?>> LoginAsync(LoginRequestDto model);
    /// <summary>
    /// Execute the refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <returns>A task  with refresh tokens</returns>
    Task<Result<(string AccessToken, string RefreshToken)?>> RefreshTokenAsync(string refreshToken);
    /// <summary>
    /// Logout: remove token
    /// </summary>
    /// <param name="userId">Current logged user</param>
    /// <returns>A task  representing the required operation</returns>
    Task<Result> LogoutAsync(string userId);
    /// <summary>
    /// Generate request for resetting password
    /// </summary>
    /// <param name="email">User email that require reset password</param>
    /// <param name="baseUrl">Base url part</param>
    /// <returns>A task  with bool result of operation</returns>
    Task<Result<bool>> ForgotPasswordAsync(string email, string baseUrl);
    /// <summary>
    /// Execute a password reset
    /// </summary>
    /// <param name="email">User email that required reset</param>
    /// <param name="token">Reset token request</param>
    /// <param name="newPassword">New password</param>
    /// <returns>A task  with bool result of operation</returns>
    Task<Result<bool>> ResetPasswordAsync(string email, string token, string newPassword);
}
