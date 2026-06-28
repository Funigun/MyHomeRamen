using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Features.ShoppingCart.Features.Common;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetPaymentDetails;

public sealed class GetPaymentDetailsValidationPolicy : AbstractValidator<GetPaymentDetailsQuery>
{
    public GetPaymentDetailsValidationPolicy(IShoppingCartDbContext dbContext)
    {
        RuleFor(x => x)
            .MustAsync(async (query, cancellationToken) =>
            {
                return await dbContext.ShoppingCarts
                    .GetByIdForUser(query.BasketId, query.UserId)
                    .AnyAsync(cancellationToken);
            })
            .WithMessage("Basket does not exist or you do not have access to it.");
    }
}

