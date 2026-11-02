namespace MyHomeRamen.Domain.Menu.MenuSections;

internal static class MenuSectionValidator
{
    internal static void ValidateMenuSection(MenuSection section)
    {
        if (section.Id.Value == Guid.Empty || section.MenuCardId.Value == Guid.Empty)
        {
            throw MenuSectionErrors.IdentifierRequired();
        }

        if (section.MenuCard is null)
        {
            throw MenuSectionErrors.MenuCardRequired();
        }

        if (string.IsNullOrWhiteSpace(section.Name))
        {
            throw MenuSectionErrors.NameRequired();
        }

        if (section.Name.Length > MenuSectionConstants.MaxNameLength)
        {
            throw MenuSectionErrors.NameTooLong();
        }

        if (section.SortOrder < MenuSectionConstants.MinSortOrder)
        {
            throw MenuSectionErrors.SortOrderInvalid();
        }
    }
}
