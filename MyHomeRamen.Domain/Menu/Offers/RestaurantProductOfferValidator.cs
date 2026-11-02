namespace MyHomeRamen.Domain.Menu.Offers;

internal static class RestaurantProductOfferValidator
{
    internal static void Validate(RestaurantProductOffer offer)
    {
        if (offer.Id.Value == Guid.Empty || offer.RestaurantId == Guid.Empty || offer.CompanyId == Guid.Empty)
        {
            throw RestaurantProductOfferErrors.IdentifierRequired();
        }

        if (offer.Product is null)
        {
            throw RestaurantProductOfferErrors.ProductRequired();
        }

        if (offer.CompanyId != offer.Product.CompanyId)
        {
            throw RestaurantProductOfferErrors.CompanyMismatch();
        }

        if (offer.Price <= 0)
        {
            throw RestaurantProductOfferErrors.PriceMustBePositive();
        }
    }
}
