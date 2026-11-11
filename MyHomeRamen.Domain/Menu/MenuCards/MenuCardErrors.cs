using MyHomeRamen.Domain.Common;

namespace MyHomeRamen.Domain.Menu.MenuCards;

public static class MenuCardErrors
{
    public static DomainException IdentifierRequired()
        => new("Menu card and restaurant identifiers must not be empty.");

    public static DomainException NameRequired()
        => new("Menu card name must not be empty.");

    public static DomainException NameTooLong()
        => new($"Menu card name must not exceed {MenuCardConstants.MaxNameLength} characters.");

    public static DomainException SortOrderInvalid()
        => new($"Menu card sort order must be greater than or equal to {MenuCardConstants.MinSortOrder}.");

    public static DomainException AlreadyActive()
        => new("Menu card is already active.");
}
