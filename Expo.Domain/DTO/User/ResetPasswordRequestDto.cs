namespace Expo.Domain.DTO.User;

/// <summary>
/// DTO to execute a reset password
/// </summary>
public class ResetPasswordRequestDto : EmailDto
{
    /// <summary>
    /// Reset password token
    /// </summary>
    public string Token { get; set; } = default!;

    /// <summary>
    /// The New password
    /// </summary>
    public string NewPassword { get; set; } = default!;
}
