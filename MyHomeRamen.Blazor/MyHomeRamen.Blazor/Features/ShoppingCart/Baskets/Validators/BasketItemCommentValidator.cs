using FluentValidation;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Validators;

public sealed class BasketItemCommentValidator : AbstractValidator<string?>
{
    public const int MaxCommentLength = 500;

    public BasketItemCommentValidator()
    {
        When(x => x is not null, () =>
        {
            RuleFor(x => x)
                .MaximumLength(MaxCommentLength)
                .WithMessage($"Comment must not exceed {MaxCommentLength} characters.");
        });
    }
}
