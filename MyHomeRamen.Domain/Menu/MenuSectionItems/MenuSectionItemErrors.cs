using MyHomeRamen.Domain.Common;

namespace MyHomeRamen.Domain.Menu.MenuSectionItems;

public static class MenuSectionItemErrors
{
    public static DomainException IdentifierRequired()
        => new("Menu section item, section, and offer identifiers must not be empty.");

    public static DomainException SectionRequired()
        => new("A menu section item must belong to a section.");

    public static DomainException OfferRequired()
        => new("A menu section item must reference an offer.");

    public static DomainException ItemNotFound()
        => new("The menu section item was not found.");

    public static DomainException WrongRestaurant()
        => new("Menu section and offer must belong to the same restaurant.");

    public static DomainException SortOrderInvalid()
        => new("Menu section item sort order must not be negative.");

    public static DomainException DuplicateOffer()
        => new("An offer cannot be listed twice in the same menu section.");
}
