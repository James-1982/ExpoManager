using Expo.Domain.DTO.User;
using FluentResults;

namespace Expo.Application.Interfaces.Services
{
/// <summary>
/// Service interface to manage users
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="email">Email of the new user</param>
    /// <param name="password">Password for the user</param>
    /// <param name="roleName">Role assigned to the user</param>
    /// <returns>Result containing the created <see cref="UserDto"/> or null</returns>
    Task<Result<UserDto?>> CreateUserAsync(string email, string password, string roleName);

    /// <summary>
    /// Promote an existing user to a higher role
    /// </summary>
    /// <param name="userId">Id of the user to promote</param>
    /// <param name="role">Role to promote to</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result<bool>> PromoteUserAsync(string userId, string role);

    /// <summary>
    /// Demote an existing user from a higher role
    /// </summary>
    /// <param name="userId">Id of the user to demote</param>
    /// <param name="role">Role to remove</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result<bool>> DemoteUserAsync(string userId, string role);

    /// <summary>
    /// Get a user by their id
    /// </summary>
    /// <param name="userId">Id of the user</param>
    /// <returns>Result containing the <see cref="UserDto"/> or null</returns>
    Task<Result<UserDto?>> GetUserByIdAsync(string userId);

    /// <summary>
    /// Get all roles assigned to a user
    /// </summary>
    /// <param name="userId">Id of the user</param>
    /// <returns>Result containing a list of role names</returns>
    Task<Result<IList<string>>> GetUserRolesAsync(string userId);

    /// <summary>
    /// Get all registered users
    /// </summary>
    /// <returns>Result containing a list of <see cref="UserDto"/></returns>
    Task<Result<IList<UserDto>>> GetAllUsersAsync();
}
}