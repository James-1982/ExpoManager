using Expo.Domain.DTO.DB;
using FluentValidation;

namespace Expo.API.Middleware.Validations.Database;

public class CategoryInDtoValidator : AbstractValidator<CategoryInDto>
{
    public CategoryInDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required")
            .MaximumLength(100)
            .WithMessage("Category name must be at most 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must be at most 500 characters");
    }
}
