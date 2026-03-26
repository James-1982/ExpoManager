using Expo.Domain.DTO.User;
using FluentResults;

namespace Expo.Domain.Interfaces.Services;

/// <summary>
/// Service to manage users
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Create a new User
    /// </summary>
    /// <param name="email">Email of new user</param>
    /// <param name="password">Password</param>
    /// <param name="roleName">Role of User</param>
    /// <returns></returns>
    Task<Result<UserDto?>> CreateUserAsync(string email, string password, string roleName);
    /// <summary>
    /// Promote an existing user to administrator role
    /// </summary>
    /// <param name="userId">User id to promote</param>
    /// <param name="role">Promoting role</param>
    /// <returns></returns>
    Task<Result<bool>> PromoteUserAsync(string userId, string role);
    /// <summary>
    /// Revoce administrator role from an user
    /// </summary>
    /// <param name="userId">User id to demote</param>
    /// <param name="role">Demoting role</param>
    /// <returns></returns>
    Task<Result<bool>> DemoteUserAsync(string userId, string role);
    /// <summary>
    /// Return an user by id
    /// </summary>
    /// <param name="userId">User id to demote</param>
    /// <returns></returns>
    Task<Result<UserDto?>> GetUserByIdAsync(string userId);
    /// <summary>
    /// Get Roles of input user
    /// </summary>
    /// <param name="userId">User id</param>
    /// <returns></returns>
    Task<Result<IList<string>>> GetUserRolesAsync(string userId);
    /// <summary>
    /// Return all existing users
    /// </summary>
    /// <returns></returns>
    Task<Result<IList<UserDto>>> GetAllUsersAsync();
}
