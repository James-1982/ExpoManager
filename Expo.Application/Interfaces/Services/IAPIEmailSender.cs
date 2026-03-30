using Expo.Application.DTO;

using FluentResults;

namespace Expo.Application.Interfaces.Services
{
/// <summary>
/// Service to send emails.
/// </summary>
/// <remarks>
/// Defines a contract for a generic service that sends emails.
/// </remarks>
public interface IAPIEmailSender
{
    /// <summary>
    /// Send an email using the provided data.
    /// </summary>
    /// <param name="emailData">DTO containing email recipient, subject, body, and optional link</param>
    /// <returns>Result indicating success or failure of the send operation</returns>
    Task<Result<bool>> SendEmailAsync(EmailObject emailData);
}
}