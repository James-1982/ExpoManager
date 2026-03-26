namespace Expo.Domain.Entities;

/// <summary>
/// Refresh token entity
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Linked token
    /// </summary>
    public string Token { get; set; }
    /// <summary>
    /// Linked user id
    /// </summary>
    public string UserId { get; set; }
    /// <summary>
    /// Expiration time
    /// </summary>
    public DateTime Expires { get; set; }
    /// <summary>
    /// Indicate if token is expired
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= Expires;
    /// <summary>
    /// Creation time
    /// </summary>
    public DateTime Created { get; set; }
    /// <summary>
    /// Revoked time
    /// </summary>
    public DateTime? Revoked { get; set; }
    /// <summary>
    /// Indicate if token is active
    /// </summary>
    public bool IsActive => Revoked == null && !IsExpired;
}