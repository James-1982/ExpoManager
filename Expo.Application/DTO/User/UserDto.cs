namespace Expo.Application.DTO.User;

/// <summary>
/// DTO representing user data
/// </summary>
public class UserDto : EmailDto
{
    /// <summary>
    /// Unique identifier of the user
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Roles assigned to the user
    /// </summary>
    public IList<string> Roles { get; set; } = new List<string>();
}