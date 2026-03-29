namespace Expo.Domain.DTO.User;

/// <summary>
/// DTO used to execute a login request
/// </summary>
public class LoginRequestDto : EmailDto
{
    /// <summary>
    /// Password of the user
    /// </summary>
    public string Password { get; set; } = default!;
}
