using FluentValidation;

namespace MyHomeRamen.Blazor.Features.Menu.Products.Components.Validators;

public sealed class ProductNameValidator : AbstractValidator<string>
{
    public const int MinLength = 5;

    public const int MaxLength = 100;

    public ProductNameValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Product name cannot be empty.")
            .MinimumLength(MinLength).WithMessage($"Product name minimum length is {MinLength}.")
            .MaximumLength(MaxLength).WithMessage($"Product name maximum length is {MaxLength}.");
    }
}
