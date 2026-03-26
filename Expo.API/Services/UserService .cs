using Expo.Domain.Constants;
using Expo.Domain.DTO.User;
using Expo.Domain.Interfaces.Services;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Expo.API.Services;

/// <summary>
/// <inheritdoc/>
/// </summary>
internal class UserService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : IUserService
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task<Result<UserDto?>> CreateUserAsync(string email, string password, string roleName)
    {
        var existing = await _userManager.FindByEmailAsync(email);

        if (existing != null)
            return Result.Fail<UserDto?>("Invalid user or password");

        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return Result.Fail<UserDto?>("Error while creating user");

        if (!await _roleManager.RoleExistsAsync(roleName))
            await _roleManager.CreateAsync(new IdentityRole(roleName));

        await _userManager.AddToRoleAsync(user, roleName);

        var permissions = RoleHierarchy.GetRolePermissions(RoleHierarchy.GetRoleByName(roleName));

        foreach (var permission in permissions)
        {
            await _userManager.AddClaimAsync(user, new Claim(permission, "true"));
        }

        var domainUser = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Roles = await _userManager.GetRolesAsync(user)
        };

        return Result.Ok(domainUser);
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task<Result<bool>> PromoteUserAsync(string userId, string promoteRoleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Fail("Invalid user");

        var currentRoles = await _userManager.GetRolesAsync(user);

        var currentRoleEnum = currentRoles
            .Select(r => Enum.TryParse<Role>(r, true, out var role) ? role : RoleHierarchy.GetMinRole())
            .DefaultIfEmpty(RoleHierarchy.GetMinRole())
            .Max();

        if (!Enum.TryParse<Role>(promoteRoleName, true, out var promoteRole))
            return Result.Fail("Invalid role");

        if (promoteRole <= currentRoleEnum)
            return Result.Ok();

        if (currentRoles.Any())
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

        await _userManager.AddToRoleAsync(user, promoteRole.ToString());

        var currentClaims = await _userManager.GetClaimsAsync(user);
        foreach (var claim in currentClaims)
        {
            await _userManager.RemoveClaimAsync(user, claim);
        }

        var newPermissions = RoleHierarchy.GetRolePermissions(promoteRole)
                                          .Select(p => new Claim(p, "true"));
        foreach (var claim in newPermissions)
        {
            await _userManager.AddClaimAsync(user, claim);
        }

        return Result.Ok();
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task<Result<bool>> DemoteUserAsync(string userId, string demoteRoleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Fail("Invalid user");

        var currentRoles = await _userManager.GetRolesAsync(user);
        var currentRoleName = currentRoles.FirstOrDefault();
        if (currentRoleName == null)
            return Result.Ok();

        if (!Enum.TryParse<Role>(currentRoleName, true, out var currentRole))
            currentRole = RoleHierarchy.GetMinRole();

        if (!Enum.TryParse<Role>(demoteRoleName, true, out var demoteRole))
            return Result.Fail("Invalid role");

        if (demoteRole >= currentRole)
            return Result.Fail("Invalid operation. Demote role is heigher than actual role");

        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        await _userManager.AddToRoleAsync(user, demoteRole.ToString());

        var currentClaims = await _userManager.GetClaimsAsync(user);
        foreach (var claim in currentClaims)
        {
            await _userManager.RemoveClaimAsync(user, claim);
        }

        var newClaims = RoleHierarchy.GetRolePermissions(demoteRole).Select(p => new Claim(p, "true"));
        foreach (var claim in newClaims)
        {
            await _userManager.AddClaimAsync(user, claim);
        }

        return Result.Ok();
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task<Result<UserDto?>> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return Result.Fail("Invalid user");

        return Result.Ok(new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Roles = await _userManager.GetRolesAsync(user)
        });
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task<Result<IList<string>>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return Result.Fail("Invalid user");

        return Result.Ok(await _userManager.GetRolesAsync(user));
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task<Result<IList<UserDto>>> GetAllUsersAsync()
    {
        var users = _userManager.Users.ToList();

        IList<UserDto> result = [];

        foreach (var u in users)
        {
            result.Add(new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Roles = await _userManager.GetRolesAsync(u)
            });
        }
        return Result.Ok(result);
    }
}
