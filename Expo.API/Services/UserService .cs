using Expo.Application.Interfaces.Services;
using Expo.Domain.Constants;
using Expo.Domain.DTO.User;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Expo.API.Services;

/// <summary>
/// Service per la gestione degli utenti
/// </summary>
internal class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Result<UserDto?>> CreateUserAsync(string email, string password, string roleName)
    {
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing != null)
            return Result.Fail<UserDto?>("User with this email already exists");

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

        // Aggiorna claims per il nuovo ruolo
        var newPermissions = RoleHierarchy.GetRolePermissions(promoteRole)
                                          .Select(p => new Claim(p, "true"));
        var currentClaims = await _userManager.GetClaimsAsync(user);
        foreach (var claim in currentClaims)
        {
            await _userManager.RemoveClaimAsync(user, claim);
        }
        foreach (var claim in newPermissions)
        {
            await _userManager.AddClaimAsync(user, claim);
        }

        return Result.Ok();
    }

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
            return Result.Fail("Invalid operation. Demote role is higher than current role");

        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, demoteRole.ToString());

        var newClaims = RoleHierarchy.GetRolePermissions(demoteRole)
                                     .Select(p => new Claim(p, "true"));
        var currentClaims = await _userManager.GetClaimsAsync(user);
        foreach (var claim in currentClaims)
        {
            await _userManager.RemoveClaimAsync(user, claim);
        }
        foreach (var claim in newClaims)
        {
            await _userManager.AddClaimAsync(user, claim);
        }

        return Result.Ok();
    }

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

    public async Task<Result<IList<string>>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Fail<IList<string>>("Invalid user");

        return Result.Ok(await _userManager.GetRolesAsync(user));
    }

    public async Task<Result<IList<UserDto>>> GetAllUsersAsync()
    {
        var users = _userManager.Users.ToList();

        var tasks = users.Select(async u => new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            Roles = await _userManager.GetRolesAsync(u)
        });

        return Result.Ok((IList<UserDto>)(await Task.WhenAll(tasks)).ToList());
    }
}