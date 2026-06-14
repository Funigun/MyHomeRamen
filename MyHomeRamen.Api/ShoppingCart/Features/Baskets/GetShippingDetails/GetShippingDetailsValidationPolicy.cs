using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetShippingDetails;

public sealed class GetShippingDetailsValidationPolicy : AbstractValidator<GetShippingDetailsQuery>
{
    public GetShippingDetailsValidationPolicy(IShoppingCartDbContext dbContext)
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
