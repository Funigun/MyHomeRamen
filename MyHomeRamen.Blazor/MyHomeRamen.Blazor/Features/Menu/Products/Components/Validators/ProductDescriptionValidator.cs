using FluentValidation;

namespace MyHomeRamen.Blazor.Features.Menu.Products.Components.Validators;

public sealed class ProductDescriptionValidator : AbstractValidator<string>
{
    public const int MinLength = 15;

    public const int MaxLength = 500;

    public ProductDescriptionValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Product description cannot be empty.")
            .MinimumLength(MinLength).WithMessage($"Product description minimum length is {MinLength}.")
            .MaximumLength(MaxLength).WithMessage($"Product description maximum length is {MaxLength}.");
    }
}
