using Expo.Domain.DTO.DB;
using FluentValidation;

namespace Expo.API.Middleware.Validations.Database;

public class StandInDtoValidator : AbstractValidator<StandInDto>
{
    public StandInDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Stand name is required")
            .MaximumLength(100)
            .WithMessage("Stand name must be at most 100 characters");

        RuleFor(x => x.Descrizione)
            .MaximumLength(500)
            .WithMessage("Description must be at most 500 characters");
    }
}
