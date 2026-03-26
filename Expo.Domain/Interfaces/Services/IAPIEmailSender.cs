using Expo.Domain.DTO;
using FluentResults;


namespace Expo.Domain.Interfaces.Services;

/// <summary>
/// Service to send email
/// </summary>
/// <remarks>
/// This interface define contract for a generic service that send email
/// </remarks>
public interface IAPIEmailSender
{
    /// <summary>
    /// Send an email with data in input object
    /// </summary>
    /// <param name="emailData">Data for sending email</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<Result<bool>> SendEmailAsync(EmailObject emailData);
}
