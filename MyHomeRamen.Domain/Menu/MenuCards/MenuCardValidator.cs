namespace MyHomeRamen.Domain.Menu.MenuCards;

internal static class MenuCardValidator
{
    internal static void ValidateMenuCard(MenuCard card)
    {
        if (card.Id.Value == Guid.Empty || card.RestaurantId == Guid.Empty)
        {
            throw MenuCardErrors.IdentifierRequired();
        }

        if (string.IsNullOrWhiteSpace(card.Name))
        {
            throw MenuCardErrors.NameRequired();
        }

        if (card.Name.Length > MenuCardConstants.MaxNameLength)
        {
            throw MenuCardErrors.NameTooLong();
        }

        ValidateSortOrder(card);
    }

    internal static void ValidateSortOrder(MenuCard card)
    {
        if (card.SortOrder < MenuCardConstants.MinSortOrder)
        {
            throw MenuCardErrors.SortOrderInvalid();
        }
    }
}
