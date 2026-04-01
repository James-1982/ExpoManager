using Expo.Domain.Constants;

namespace Expo.Application.DTO.User
{
    /// <summary>
    /// DTO to execute the registration of a new user
    /// </summary>
    public class RegisterRequestDto : EmailDto
    {
        /// <summary>
        /// Password for the new user
        /// </summary>
        public string Password { get; set; } = default!;
    }

    /// <summary>
    /// DTO used by admin to create a new user with a specific role
    /// </summary>
    public class RegisterUserDto : RegisterRequestDto
    {
        /// <summary>
        /// Role to assign to the new user
        /// </summary>
        public string Role { get; set; } = RoleHierarchy.GetRoleName(Domain.Constants.Role.Supervisor)!;
    }
}