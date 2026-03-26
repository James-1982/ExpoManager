using Expo.Domain.Constants;
using Expo.Domain.DTO.DB;
using Expo.Domain.DTO.User;
using FluentValidation;

namespace Expo.API.Middleware.Validations.Authentication;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long");
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequestDto>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Reset token is required");
    }
}

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequestDto>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotNull()
            .NotEmpty().WithMessage("Invalid refresh token");
    }
}

/// <summary>
/// User managment from Admin
/// </summary>
public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long");

        RuleFor(x => x.Role)
            .NotEmpty()
            .NotNull()
            .WithMessage("Role can not be null or empty")
            .Must(BeAValidRole).WithMessage("Inserted role is not valid");
    }

    private bool BeAValidRole(string? roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return false;

        return Enum.TryParse<Role>(roleName, true, out _);
    }
}


public class ChangeRoleValidator : AbstractValidator<ChangeRoleDto>
{
    public ChangeRoleValidator()
    {
        RuleFor(x => x.NewRole)
            .NotNull()
            .WithMessage("Il ruolo non può essere vuoto.")
            .Must(BeAValidRole).WithMessage("Il ruolo specificato non è valido.");
    }

    private bool BeAValidRole(string? roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return false;

        return Enum.TryParse<Role>(roleName, true, out _);
    }
}