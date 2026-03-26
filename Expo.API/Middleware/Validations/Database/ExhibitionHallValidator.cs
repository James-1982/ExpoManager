using Expo.Domain.DTO.DB;
using FluentValidation;

namespace Expo.API.Middleware.Validations.Database;

public class ExhibitionHallInDtoValidator : AbstractValidator<ExhibitionHallInDto>
{
    public ExhibitionHallInDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("ExhibitionHall name is required")
            .MaximumLength(100)
            .WithMessage("ExhibitionHall name must be at most 100 characters");

        RuleFor(x => x.Descrizione)
            .MaximumLength(500)
            .WithMessage("Description must be at most 500 characters");

        RuleFor(x => x.Stato)
           .NotNull()
           .WithMessage("State is mandatory")
           .IsInEnum()
           .WithMessage("Invalid state");

    }
}