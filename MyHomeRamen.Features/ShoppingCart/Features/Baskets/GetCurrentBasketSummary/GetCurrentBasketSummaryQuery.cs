using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetCurrentBasketSummary;

public sealed record GetCurrentBasketSummaryQuery : IQuery<GetCurrentBasketSummaryResponse>;

public sealed class GetCurrentBasketSummaryHandler(IShoppingCartDbContext dbContext, ICurrentUser currentUser)
                  : IQueryHandler<GetCurrentBasketSummaryQuery, GetCurrentBasketSummaryResponse>
{
    public async Task<GetCurrentBasketSummaryResponse> Handle(GetCurrentBasketSummaryQuery request, CancellationToken cancellationToken)
    {
        User? user = await dbContext.User.Query().FindByIdAsync((UserId)currentUser.UserId, cancellationToken);

        if (user is null ||
             (user.IsGuest && !string.IsNullOrEmpty(currentUser.Id))
              || (!user.IsGuest && string.IsNullOrEmpty(currentUser.Id))
            )
        {
            throw new UnauthorizedAccessException("User is not authorized to access the current basket summary.");
        }

        return (await dbContext.Basket.Query().GetCurrentBasketSummaryAsync(currentUser.UserId, cancellationToken))!
            .ToResponse();
    }
}

