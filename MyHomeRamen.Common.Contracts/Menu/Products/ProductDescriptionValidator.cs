using FluentValidation;

namespace MyHomeRamen.Common.Contracts.Menu.Products;

public sealed class ProductDescriptionValidator : AbstractValidator<string>
{
    public const int MinLength = 50;

    public const int MaxLength = 500;

    public ProductDescriptionValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .MinimumLength(MinLength)
            .MaximumLength(MaxLength);
    }
}
