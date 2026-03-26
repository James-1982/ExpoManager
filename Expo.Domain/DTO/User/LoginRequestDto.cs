namespace Expo.Domain.DTO.User;

/// <summary>
/// DTO for execute a login request
/// </summary>
public class LoginRequestDto : EmailDto
{
    /// <summary>
    /// The password
    /// </summary>
    public string Password { get; set; } = default!;
}
