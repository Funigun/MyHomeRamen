using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Features.Menu.Features.Offers.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class RestaurantProductOfferRepository : IRestaurantProductOfferLoader
{
    public async Task<RestaurantProductOffer?> ByIdForUpdate(
        RestaurantProductOfferId offerId,
        CancellationToken cancellationToken)
        => await menuDbContext.RestaurantProductOffers
            .Include(offer => offer.Product)
                .ThenInclude(product => product.BaseIngredients)
            .Include(offer => offer.Product)
                .ThenInclude(product => product.CustomIngredients)
            .FirstOrDefaultAsync(offer => offer.Id == offerId, cancellationToken);
}
