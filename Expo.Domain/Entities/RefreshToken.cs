namespace Expo.Domain.Entities;

/// <summary>
/// Refresh token entity for authentication
/// </summary>
public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; }
    public string UserId { get; set; }
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Revoked { get; set; }

    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsActive => Revoked == null && !IsExpired;

    public RefreshToken() { }

    public RefreshToken(string token, string userId, DateTime expires)
    {
        Token = token ?? throw new ArgumentNullException(nameof(token));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Expires = expires;
        Created = DateTime.UtcNow;
    }

    public void Revoke() => Revoked = DateTime.UtcNow;
}