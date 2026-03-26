namespace Expo.Domain.DTO.User;

/// <summary>
/// DTO to execute the registration of new user
/// </summary>
public class RegisterRequestDto : EmailDto
{
    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; } = default!;
}

/// <summary>
/// DTO used by admin to create a new user
/// </summary>
public class RegisterUserDto : RegisterRequestDto
{
    /// <summary>
    /// Role for new user
    /// </summary>
    public string Role { get; set; } = default!;
}
