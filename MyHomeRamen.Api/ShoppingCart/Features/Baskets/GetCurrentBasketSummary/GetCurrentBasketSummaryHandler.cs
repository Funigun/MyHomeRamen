using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketSummary;

public sealed class GetCurrentBasketSummaryHandler(IShoppingCartDbContext dbContext, ICurrentUser currentUser)
                  : IQueryHandler<GetCurrentBasketSummaryQuery, GetCurrentBasketSummaryResponse>
{
    public async Task<GetCurrentBasketSummaryResponse> Handle(GetCurrentBasketSummaryQuery request, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users.AsNoTracking()
                                             .Where(u => u.Id == (UserId)currentUser.UserId)
                                             .FirstOrDefaultAsync(cancellationToken);

        if (user is null ||
             (user.IsGuest && !string.IsNullOrEmpty(currentUser.Id))
              || (!user.IsGuest && string.IsNullOrEmpty(currentUser.Id))
            )
        {
            throw new UnauthorizedAccessException("User is not authorized to access the current basket summary.");
        }

        return await dbContext.ShoppingCarts.GetCurrentBasketSummary(currentUser.UserId)
                                            .Select(basket => basket.ToResponse())
                                            .FirstAsync(cancellationToken);
    }
}
