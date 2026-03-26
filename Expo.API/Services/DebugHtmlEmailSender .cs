using Expo.Domain.DTO;
using Expo.Domain.Interfaces.Services;
using FluentResults;
using System.Net;

namespace Expo.API.Services;


/// <summary>
/// <inheritdoc/>
/// </summary>
///<remarks>
/// Visual simulation email sender
///</remarks>
internal class DebugHtmlEmailSender : IAPIEmailSender
{
    private readonly string _filePath = Path.Combine(Path.GetTempPath(), "EmailSimulator.html");

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<Result<bool>> SendEmailAsync(EmailObject email)
    {
        if (File.Exists(_filePath))
            File.Delete(_filePath);

        // Genera il contenuto HTML
        var content = $@"
            <div style='font-family: Arial, sans-serif; display: flex; flex-direction: column; align-items: center; text-align: center; margin: 40px;'>
                <h2 style='font-size: 1.5em; margin-bottom: 20px;'>{WebUtility.HtmlEncode(email.Email)}</h2>
                <p style='font-size: 1em; margin-bottom: 15px;'>{WebUtility.HtmlEncode(email.Subject)}</p>
                <a href='{email.Link}' style='font-size: 1em; color: #007bff; text-decoration: none;' target='_blank'>Click here</a>
                <hr style='margin-top: 30px; width: 80%;'/>
            </div>";

        // Appendi al file esistente
        File.AppendAllText(_filePath, content);

        // Apri il file nel browser
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = _filePath,
            UseShellExecute = true
        });

        return Task.FromResult(Result.Ok(true));
    }
}
