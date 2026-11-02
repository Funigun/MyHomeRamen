namespace MyHomeRamen.Domain.Menu.MenuSectionItems;

internal static class MenuSectionItemValidator
{
    internal static void ValidateMenuSectionItem(MenuSectionItem item)
    {
        if (item.Id.Value == Guid.Empty
            || item.MenuSectionId.Value == Guid.Empty
            || item.RestaurantProductOfferId.Value == Guid.Empty)
        {
            throw MenuSectionItemErrors.IdentifierRequired();
        }

        if (item.MenuSection is null)
        {
            throw MenuSectionItemErrors.SectionRequired();
        }

        if (item.RestaurantProductOffer is null)
        {
            throw MenuSectionItemErrors.OfferRequired();
        }

        if (item.MenuSection.MenuCard.RestaurantId != item.RestaurantProductOffer.RestaurantId)
        {
            throw MenuSectionItemErrors.WrongRestaurant();
        }

        if (item.SortOrder < 0)
        {
            throw MenuSectionItemErrors.SortOrderInvalid();
        }
    }
}
