using FluentValidation;

namespace MyHomeRamen.Common.Contracts.Menu.Products;

public sealed class ProductNameValidator : AbstractValidator<string>
{
    public const int MinLength = 15;

    public const int MaxLength = 100;

    public ProductNameValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .MinimumLength(MinLength)
            .MaximumLength(MaxLength);
    }
}
