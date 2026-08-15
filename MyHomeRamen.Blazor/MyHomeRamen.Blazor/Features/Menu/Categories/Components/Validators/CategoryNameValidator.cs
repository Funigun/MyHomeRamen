using FluentValidation;

namespace MyHomeRamen.Blazor.Features.Menu.Categories.Components.Validators;

public sealed class CategoryNameValidator : AbstractValidator<string>
{
    public const int MinLength = 3;
    public const int MaxLength = 50;

    public CategoryNameValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Category name cannot be empty.")
            .MinimumLength(MinLength).WithMessage($"Category name minimum length is {MinLength}.")
            .MaximumLength(MaxLength).WithMessage($"Category name maximum length is {MaxLength}.");
    }
}
