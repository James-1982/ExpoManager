using Expo.Domain.DTO.DB;
using FluentValidation;

namespace Expo.API.Middleware.Validations.Database;

public class PavilionInDtoValidator : AbstractValidator<PavilionInDto>
{
    public PavilionInDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Pavilion name is required")
            .MaximumLength(100)
            .WithMessage("Pavilion name must be at most 100 characters");

        RuleFor(x => x.Descrizione)
            .MaximumLength(500)
            .WithMessage("Description must be at most 500 characters");
    }
}

