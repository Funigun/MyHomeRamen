using MyHomeRamen.Domain.Common;

namespace MyHomeRamen.Domain.Menu.MenuSections;

public static class MenuSectionErrors
{
    public static DomainException IdentifierRequired()
        => new("Menu section and menu card identifiers must not be empty.");

    public static DomainException MenuCardRequired()
        => new("A menu section must belong to a menu card.");

    public static DomainException NameRequired()
        => new("Menu section name must not be empty.");

    public static DomainException NameTooLong()
        => new($"Menu section name must not exceed {MenuSectionConstants.MaxNameLength} characters.");

    public static DomainException SortOrderInvalid()
        => new("Menu section sort order must not be negative.");
}
