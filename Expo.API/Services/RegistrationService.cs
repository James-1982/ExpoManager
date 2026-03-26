using Expo.API.Utils;
using Expo.Domain.DTO;
using Expo.Domain.Interfaces.Services;
using FluentResults;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Expo.API.Services;

/// <summary>
/// <inheritdoc/>
/// </summary>
///<remarks>
/// Real email sender using SMTP
///</remarks>
internal class RegistationService(
    IOptions<EmailSettings> settings,
    ILogger<RegistationService> logger) : IAPIEmailSender
{
    private readonly EmailSettings _settings = settings.Value;
    private readonly ILogger<RegistationService> _logger = logger;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task<Result<bool>> SendEmailAsync(EmailObject emailData)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation($"Email sending disabled, skipping sending to {emailData.Email}");
            Console.WriteLine($"Link: {emailData.Link}");
            return Result.Fail($"Email sending disabled, skipping sending to {emailData.Email}");
        }

        try
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = _settings.EnableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.From),
                Subject = emailData.Subject,
                Body = emailData.Bodby,
                IsBodyHtml = true
            };
            mailMessage.To.Add(emailData.Email);

            await client.SendMailAsync(mailMessage);

            _logger.LogInformation($"Email sent to {emailData.Email} with subject '{emailData.Subject}'");

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email to {emailData.Email}");
            return Result.Fail(ex.ToString());
        }
    }
}
