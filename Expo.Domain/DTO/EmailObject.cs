namespace Expo.Domain.DTO;

/// <summary>
/// Object to send Data
/// </summary>
public class EmailObject
{
    /// <summary>
    /// Email Receiver
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Subject of email
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// Email body
    /// </summary>
    public string Bodby { get; set; }

    /// <summary>
    /// Link
    /// </summary>
    public string Link { get; set; }
}
