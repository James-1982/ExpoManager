using Expo.Application.DTO.DB;
using FluentValidation;

namespace Expo.API.Middleware.Validations.Database;

public class ExhibitionHallInDtoValidator : AbstractValidator<ExhibitionAreaInDto>
{
    public ExhibitionHallInDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("ExhibitionHall name is required")
            .MaximumLength(100)
            .WithMessage("ExhibitionHall name must be at most 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must be at most 500 characters");

        RuleFor(x => x.State)
           .NotNull()
           .WithMessage("State is mandatory")
           .IsInEnum()
           .WithMessage("Invalid state");

    }
}