using FluentValidation;

namespace MyHomeRamen.Common.Contracts.Basket;

public sealed class BasketItemQuantityValidator : AbstractValidator<int>
{
    public const int MinQuantity = 1;

    public const int MaxQuantity = 50;

    public BasketItemQuantityValidator()
    {
        RuleFor(x => x)
            .GreaterThanOrEqualTo(MinQuantity)
            .WithMessage($"Quantity must be greater than or equal to {MinQuantity}.")
            .LessThanOrEqualTo(MaxQuantity)
            .WithMessage($"Quantity must be less than or equal to {MaxQuantity}.");
    }
}
