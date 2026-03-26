namespace Expo.Domain.DTO.User;

/// <summary>
/// DTO with User data
/// </summary>
public class UserDto : EmailDto
{
    /// <summary>
    /// Unique Id
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Role linked to current User
    /// </summary>
    public IList<string> Roles { get; set; } = [];
}