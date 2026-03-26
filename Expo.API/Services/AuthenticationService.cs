using Expo.Domain.Constants;
using Expo.Domain.DTO;
using Expo.Domain.DTO.User;
using Expo.Domain.Entities;
using Expo.Domain.Interfaces.Repo;
using Expo.Domain.Interfaces.Services;
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
/// Authentication service
/// </summary>
/// <param name="userManager">Identity user manager.</param>
/// <param name="signInManager">Identity sign in manager.</param>
/// <param name="roleManager">Identity role manager.</param>
/// <param name="uow">Unit of Work for transactional persistence.</param>
/// <param name="emailSender">Service to send email.</param>
/// <param name="configuration">Web API configuration structure.</param>
/// <remarks>
/// This constructor injects all required services for category operations.
/// </remarks>
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
    /// <summary>
    /// <inheritdoc></inheritdoc>/>
    /// </summary>
    public async Task<Result<bool>> RegisterAsync(RegisterUserDto model, string confirmationLinkBaseUrl)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
                return Result.Fail("User registered");

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = false
            };

            var identityResult = await _userManager.CreateAsync(user, model.Password);

            if (!identityResult.Succeeded)
                return Result.Fail(identityResult.ToString());

            // Ruoli
            var roleName = model.Role;
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));

            await _userManager.AddToRoleAsync(user, roleName);

            // Claims di default
            var permissions = RoleHierarchy.GetRolePermissions(roleName);

            var roles = await _roleManager.FindByNameAsync(roleName);

            var roleClaims = await _roleManager.GetClaimsAsync(roles);

            foreach (var permission in permissions)
            {
                var claim = new Claim(permission, "true");

                if (!roleClaims.Any(c => c.Type == claim.Type))
                    await _userManager.AddClaimAsync(user, claim);
            }

            // confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var tokenBytes = System.Text.Encoding.UTF8.GetBytes(token);
            var encodedToken = WebEncoders.Base64UrlEncode(tokenBytes);

            // confirmation link
            var confirmationLink = $"{confirmationLinkBaseUrl}?userId={user.Id}&token={encodedToken}";

            //send confirmation email
            var emailObject = new EmailObject()
            {
                Email = user.Email,
                Subject = "Confirm you registration",
                Link = confirmationLink,
                Bodby = $"Click here <a href='{confirmationLink}'>qui</a> to confirm registration"
            };

            await _emailSender.SendEmailAsync(emailObject);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
    /// <summary>
    /// <inheritdoc></inheritdoc>/>
    /// </summary>
    public async Task<Result<bool>> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Fail("Invalid user");

        // Decodifica URL-safe Base64
        var tokenBytes = WebEncoders.Base64UrlDecode(token);

        var decodedToken = System.Text.Encoding.UTF8.GetString(tokenBytes);

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        return result.Succeeded
            ? Result.Ok()
            : Result.Fail("Error confirming user");
    }
    /// <summary>
    /// <inheritdoc></inheritdoc>/>
    /// </summary>
    public async Task<Result<(string Token, string RefreshToken, DateTime Expiration)?>> LoginAsync(LoginRequestDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Result.Fail<(string, string, DateTime)?>("Invalid user");

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
            return Result.Fail<(string, string, DateTime)?>(result.ToString());

        // 1. genera access token
        var jwtToken = await GetTokenAsync(user);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        // 2. genera refresh token
        var refreshToken = GenerateRefreshToken();

        // 3. salva nel DB tramite UnitOfWork
        await _uow.RefreshToken.AddAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        await _uow.SaveAsync();

        return Result.Ok<(string, string, DateTime)?>((accessToken, refreshToken, jwtToken.ValidTo));
    }
    /// <summary>
    /// <inheritdoc></inheritdoc>/>
    /// </summary>
    public async Task<Result<(string AccessToken, string RefreshToken)?>> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _uow.RefreshToken.GetByTokenAsync(refreshToken);

        if (storedToken == null || !storedToken.IsActive)
            return Result.Fail<(string, string)?>("Invalid request");

        var user = await _userManager.FindByIdAsync(storedToken.UserId);
        if (user == null)
            return Result.Fail<(string, string)?>("Invalid user");

        // invalida il token vecchio
        storedToken.Revoked = DateTime.UtcNow;

        // genera nuovi token
        var jwtToken = await GetTokenAsync(user);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        var newRefresh = GenerateRefreshToken();

        // salva nuovo refresh
        await _uow.RefreshToken.AddAsync(new RefreshToken
        {
            Token = newRefresh,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        await _uow.SaveAsync();

        return Result.Ok<(string, string)?>((accessToken, newRefresh));
    }
    /// <summary>
    /// <inheritdoc></inheritdoc>/>
    /// </summary>
    public async Task<Result> LogoutAsync(string userId)
    {
        // Recupera tutti i refresh token attivi dell'utente
        var tokens = await _uow.RefreshToken
            .GetAllAsync(t => t.UserId == userId && t.IsActive);

        foreach (var token in tokens)
        {
            token.Revoked = DateTime.UtcNow; // Segna il token come revocato
            _uow.RefreshToken.Update(token); // Aggiorna il token
        }

        await _uow.SaveAsync();

        return Result.Ok();
    }
    /// <summary>
    /// <inheritdoc></inheritdoc>/>
    /// </summary>
    public async Task<Result<bool>> ForgotPasswordAsync(string email, string baseUrl)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Result.Fail($"Missing user");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var tokenBytes = System.Text.Encoding.UTF8.GetBytes(token);

        var encodedToken = WebEncoders.Base64UrlEncode(tokenBytes);


        // Se baseUrl contiene già ?, aggiungi con & altrimenti ?
        var separator = baseUrl.Contains('?') ? "&" : "?";
        var resetLink = $"{baseUrl}{separator}email={WebUtility.UrlEncode(email)}&token={encodedToken}";

        var emailObject = new EmailObject()
        {
            Email = email,
            Subject = "Reset Password",
            Link = resetLink,
            Bodby = $"Click here <a href='{resetLink}'>here</a> to reset password"
        };

        await _emailSender.SendEmailAsync(emailObject);

        return Result.Ok();
    }
    /// <summary>
    /// <inheritdoc></inheritdoc>/>
    /// </summary>
    public async Task<Result<bool>> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return Result.Fail($"Missing user");

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        return result.Succeeded
            ? Result.Ok()
            : Result.Fail("Error while resetting password");
    }
    /// <summary>
    /// <inheritdoc></inheritdoc>/>
    /// </summary>
    private async Task<JwtSecurityToken> GetTokenAsync(IdentityUser user, IEnumerable<Claim>? extraClaims = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(await _userManager.GetClaimsAsync(user));

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var roleName in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleName));

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) continue;

            claims.AddRange(await _roleManager.GetClaimsAsync(role));
        }

        if (extraClaims != null)
            claims.AddRange(extraClaims);

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
    /// <summary>
    /// <inheritdoc></inheritdoc>/>
    /// </summary>
    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes);
    }
}
