using Expo.Application.Interfaces.Services;
using Expo.Domain.Constants;
using Expo.Domain.DTO;
using Expo.Domain.DTO.User;
using Expo.Domain.Entities;
using Expo.Domain.Interfaces.Repositories;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Expo.API.Services;

/// <summary>
/// Service to handle authentication, registration, login, token management and password reset.
/// </summary>
internal class AuthenticationService(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    RoleManager<IdentityRole> roleManager,
    IUnitOfWork uow,
    IAPIEmailSender emailSender,
    IConfiguration configuration) : IAuthenticationService
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly IConfiguration _configuration = configuration;
    private readonly IAPIEmailSender _emailSender = emailSender;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Result<bool>> RegisterAsync(RegisterUserDto model, string confirmationLinkBaseUrl)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return Result.Fail("User already registered");

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = false
            };

            var identityResult = await _userManager.CreateAsync(user, model.Password);
            if (!identityResult.Succeeded)
                return Result.Fail(identityResult.ToString());

            if (!await _roleManager.RoleExistsAsync(model.Role))
                await _roleManager.CreateAsync(new IdentityRole(model.Role));

            await _userManager.AddToRoleAsync(user, model.Role);

            var permissions = RoleHierarchy.GetRolePermissions(model.Role);
            var role = await _roleManager.FindByNameAsync(model.Role);
            var roleClaims = await _roleManager.GetClaimsAsync(role);

            foreach (var permission in permissions)
            {
                var claim = new Claim(permission, "true");
                if (!roleClaims.Any(c => c.Type == claim.Type))
                    await _userManager.AddClaimAsync(user, claim);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(token));
            var confirmationLink = $"{confirmationLinkBaseUrl}?userId={user.Id}&token={encodedToken}";

            await _emailSender.SendEmailAsync(new EmailObject
            {
                Email = user.Email,
                Subject = "Confirm your registration",
                Link = confirmationLink,
                Body = $"Click <a href='{confirmationLink}'>here</a> to confirm registration"
            });

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    public async Task<Result<bool>> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result.Fail("Invalid user");

        var decodedToken = System.Text.Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        return result.Succeeded ? Result.Ok() : Result.Fail("Error confirming user");
    }

    public async Task<Result<(string AccessToken, string RefreshToken, DateTime Expiration)?>> LoginAsync(LoginRequestDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return Result.Fail<(string, string, DateTime)?>("Invalid user");

        var check = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!check.Succeeded) return Result.Fail<(string, string, DateTime)?>(check.ToString());

        var jwtToken = await GetTokenAsync(user);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        var refreshToken = GenerateRefreshToken();

        await _uow.RefreshTokens.AddAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        await _uow.SaveAsync();

        return Result.Ok<(string, string, DateTime)?>((accessToken, refreshToken, jwtToken.ValidTo));
    }

    public async Task<Result<(string AccessToken, string RefreshToken)?>> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _uow.RefreshTokens.GetByTokenAsync(refreshToken);
        if (storedToken == null || !storedToken.IsActive) return Result.Fail<(string, string)?>("Invalid request");

        var user = await _userManager.FindByIdAsync(storedToken.UserId);
        if (user == null) return Result.Fail<(string, string)?>("Invalid user");

        storedToken.Revoked = DateTime.UtcNow;

        var jwtToken = await GetTokenAsync(user);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        var newRefreshToken = GenerateRefreshToken();

        await _uow.RefreshTokens.AddAsync(new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        await _uow.SaveAsync();

        return Result.Ok<(string, string)?>((accessToken, newRefreshToken));
    }

    public async Task<Result> LogoutAsync(string userId)
    {
        var tokens = await _uow.RefreshTokens.GetAllAsync(t => t.UserId == userId && t.IsActive);
        foreach (var token in tokens)
        {
            token.Revoked = DateTime.UtcNow;
            _uow.RefreshTokens.Update(token);
        }
        await _uow.SaveAsync();
        return Result.Ok();
    }

    public async Task<Result<bool>> ForgotPasswordAsync(string email, string baseUrl)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return Result.Fail("Missing user");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(token));

        var separator = baseUrl.Contains('?') ? "&" : "?";
        var resetLink = $"{baseUrl}{separator}email={WebUtility.UrlEncode(email)}&token={encodedToken}";

        await _emailSender.SendEmailAsync(new EmailObject
        {
            Email = email,
            Subject = "Reset Password",
            Link = resetLink,
            Body = $"Click <a href='{resetLink}'>here</a> to reset password"
        });

        return Result.Ok();
    }

    public async Task<Result<bool>> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return Result.Fail("Missing user");

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded ? Result.Ok() : Result.Fail("Error while resetting password");
    }

    private async Task<JwtSecurityToken> GetTokenAsync(IdentityUser user, IEnumerable<Claim>? extraClaims = null)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(await _userManager.GetClaimsAsync(user));

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var roleName in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleName));
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
                claims.AddRange(await _roleManager.GetClaimsAsync(role));
        }

        if (extraClaims != null) claims.AddRange(extraClaims);

        var jwtSection = _configuration.GetSection("Jwt");
        var keyBytes = Convert.FromBase64String(jwtSection["Key"]);
        var signingKey = new SymmetricSecurityKey(keyBytes);

        return new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSection["AccessTokenExpiration"])),
            claims: claims,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        );
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}