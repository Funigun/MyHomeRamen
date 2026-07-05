using FluentValidation;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetPaymentDetails;

public sealed class GetPaymentDetailsValidationPolicy : AbstractValidator<GetPaymentDetailsQuery>
{
    public GetPaymentDetailsValidationPolicy(IShoppingCartDbContext dbContext)
    {
        RuleFor(x => x)
            .MustAsync(async (query, cancellationToken) =>
            {
                return await dbContext.Basket.Query()
                    .GetByIdForUserAsync(query.BasketId, query.UserId, cancellationToken) != null;
            })
            .WithMessage("Basket does not exist or you do not have access to it.");
    }
}

