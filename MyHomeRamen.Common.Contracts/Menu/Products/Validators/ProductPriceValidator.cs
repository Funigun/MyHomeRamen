using FluentValidation;

namespace MyHomeRamen.Common.Contracts.Menu.Products.Validators;

public sealed class ProductPriceValidator : AbstractValidator<decimal>
{
    public const decimal MinPrice = 0.5m;

    public const decimal MaxPrice = 100.0m;

    public ProductPriceValidator()
    {
        RuleFor(x => x)
            .LessThanOrEqualTo(MaxPrice).WithMessage($"Product price cannot be greater than or equal to {MaxPrice}.")
            .GreaterThanOrEqualTo(MinPrice).WithMessage($"Product price cannot be less than or equal to {MinPrice}");
    }
}
