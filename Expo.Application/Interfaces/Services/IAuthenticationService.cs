using Expo.Application.DTO.User;
using FluentResults;

namespace Expo.Application.Interfaces.Services
{
/// <summary>
/// Service to handle user authentication and authorization.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Register a new user with specified role.
    /// </summary>
    /// <param name="model">Input data for registering a new user</param>
    /// <param name="confirmationLinkBaseUrl">Base URL for email confirmation link</param>
    /// <returns>Result indicating success or failure of the operation</returns>
    Task<Result<bool>> RegisterAsync(RegisterUserDto model, string confirmationLinkBaseUrl);

    /// <summary>
    /// Confirm a user's email registration.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="token">Email confirmation token</param>
    /// <returns>Result indicating success or failure of confirmation</returns>
    Task<Result<bool>> ConfirmEmailAsync(string userId, string token);

    /// <summary>
    /// Authenticate user and return access and refresh tokens.
    /// </summary>
    /// <param name="model">Login input data</param>
    /// <returns>Tokens and expiration if login is successful</returns>
    Task<Result<(string AccessToken, string RefreshToken, DateTime Expiration)?>> LoginAsync(LoginRequestDto model);

    /// <summary>
    /// Refresh the access token using a valid refresh token.
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <returns>New access and refresh tokens</returns>
    Task<Result<(string AccessToken, string RefreshToken)?>> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Logout a user by revoking tokens.
    /// </summary>
    /// <param name="userId">Current logged-in user ID</param>
    /// <returns>Result of the operation</returns>
    Task<Result> LogoutAsync(string userId);

    /// <summary>
    /// Generate a password reset request for a user.
    /// </summary>
    /// <param name="email">User email requiring password reset</param>
    /// <param name="baseUrl">Base URL for reset link</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result<bool>> ForgotPasswordAsync(string email, string baseUrl);

    /// <summary>
    /// Reset a user's password using a reset token.
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="token">Reset token</param>
    /// <param name="newPassword">New password</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result<bool>> ResetPasswordAsync(string email, string token, string newPassword);
}
}