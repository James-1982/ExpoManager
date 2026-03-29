namespace Expo.Domain.DTO.User;

/// <summary>
/// DTO used to change the role of a user
/// </summary>
public class ChangeRoleDto
{
    /// <summary>
    /// New role to assign to the user
    /// </summary>
    public string? NewRole { get; set; }
}