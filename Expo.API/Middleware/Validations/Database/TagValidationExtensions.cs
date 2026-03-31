using FluentValidation;

namespace Expo.API.Middleware.Validations.Database;

public static class TagValidationExtensions
{
    public static IRuleBuilderOptions<T, IEnumerable<string>> ValidateTags<T>(
        this IRuleBuilder<T, IEnumerable<string>> ruleBuilder)
    {
        return ruleBuilder
            .NotNull().WithMessage("Tags cannot be null")
            .Must(tags => tags.Count() <= 10)
                .WithMessage("You can specify at most 10 tags")
            .ForEach(tagRule => tagRule
                .MaximumLength(30).WithMessage("Each tag must be at most 30 characters"));
    }
}