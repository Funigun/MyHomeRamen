using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Common;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetCurrentBasketDetails;

public sealed class GetCurrentBasketDetailsHandler(IShoppingCartDbContext dbContext, ICurrentUser currentUser)
    : IQueryHandler<GetCurrentBasketDetailsQuery, GetCurrentBasketDetailsResponse?>
{
    public async Task<GetCurrentBasketDetailsResponse?> Handle(GetCurrentBasketDetailsQuery request, CancellationToken cancellationToken)
    {
        UserId userId = new(currentUser.UserId);

        Basket? basket = await dbContext.ShoppingCarts
            .ForUser(userId)
            .FirstOrDefaultAsync(cancellationToken);

        return basket?.ToResponse();
    }
}

