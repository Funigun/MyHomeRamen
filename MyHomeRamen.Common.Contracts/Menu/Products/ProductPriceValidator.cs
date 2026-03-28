using FluentValidation;

namespace MyHomeRamen.Common.Contracts.Menu.Products;

public sealed class ProductPriceValidator : AbstractValidator<decimal>
{
    public const decimal MinPrice = 0.5m;

    public const decimal MaxPrice = 100.0m;

    public ProductPriceValidator()
    {
        RuleFor(x => x)
            .GreaterThanOrEqualTo(MinPrice)
            .LessThanOrEqualTo(MaxPrice);
    }
}
