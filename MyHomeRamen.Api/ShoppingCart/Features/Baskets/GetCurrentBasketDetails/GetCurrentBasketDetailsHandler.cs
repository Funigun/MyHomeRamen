using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketDetails.Models;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketDetails;

public sealed class GetCurrentBasketDetailsHandler(IShoppingCartDbContext dbContext, ICurrentUser currentUser)
    : IRequestHandler<GetCurrentBasketDetailsRequest, GetCurrentBasketDetailsResponse?>
{
    public async Task<GetCurrentBasketDetailsResponse?> Handle(GetCurrentBasketDetailsRequest request, CancellationToken cancellationToken)
    {
        UserId userId = new(currentUser.UserId);

        Basket? basket = await dbContext.ShoppingCarts
            .ForUser(userId)
            .FirstOrDefaultAsync(cancellationToken);

        return basket?.ToResponse();
    }
}
