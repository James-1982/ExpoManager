namespace Expo.API.Utils;

/// <summary>
/// Email setting 
/// </summary>
public class EmailSettings
{
    /// <summary>
    /// Enabled
    /// </summary>
    public bool Enabled { get; set; }
    /// <summary>
    /// Host service
    /// </summary>
    public string Host { get; set; } = string.Empty;
    /// <summary>
    /// Port
    /// </summary>
    public int Port { get; set; } = 587;
    /// <summary>
    /// EnableSsl
    /// </summary>
    public bool EnableSsl { get; set; } = true;
    /// <summary>
    /// From
    /// </summary>
    public string From { get; set; } = string.Empty;
    /// <summary>
    /// Username
    /// </summary>
    public string Username { get; set; } = string.Empty;
    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; } = string.Empty;
}