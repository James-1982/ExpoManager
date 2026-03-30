namespace Expo.Application.DTO.User;

/// <summary>
/// DTO used to execute a password reset request
/// </summary>
public class ResetPasswordRequestDto : EmailDto
{
    /// <summary>
    /// Token used for resetting the password
    /// </summary>
    public string Token { get; set; } = default!;

    /// <summary>
    /// The new password to set
    /// </summary>
    public string NewPassword { get; set; } = default!;
}
