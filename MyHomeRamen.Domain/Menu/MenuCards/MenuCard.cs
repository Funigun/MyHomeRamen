using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Menu.MenuSections;

namespace MyHomeRamen.Domain.Menu.MenuCards;

public sealed class MenuCard : AuditableEntity, IEntity<MenuCardId>
{
    private readonly List<MenuSection> _sections = [];

    public MenuCardId Id { get; private set; }

    public Guid RestaurantId { get; private set; }

    public string Name { get; private set; } = default!;

    public bool IsActive { get; private set; }

    public int SortOrder { get; private set; }

    public IReadOnlyCollection<MenuSection> Sections => _sections.AsReadOnly();

    private MenuCard() { }

    private MenuCard(MenuCardId id, Guid restaurantId, string name, int sortOrder)
    {
        Id = id;
        RestaurantId = restaurantId;
        Name = name;
        IsActive = true;
        SortOrder = sortOrder;
    }

    public static MenuCard Create(
        MenuCardId id,
        Guid restaurantId,
        string name,
        int sortOrder = MenuCardConstants.MinSortOrder)
    {
        MenuCard card = new(id, restaurantId, name, sortOrder);
        MenuCardValidator.ValidateMenuCard(card);
        return card;
    }

    public void UpdateName(string name)
    {
        Name = name;
        MenuCardValidator.ValidateMenuCard(this);
    }

    public void SetActive(bool isActive) => IsActive = isActive;

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        MenuCardValidator.ValidateSortOrder(this);
    }

    public void Activate()
    {
        if (IsActive)
        {
            throw MenuCardErrors.AlreadyActive();
        }

        IsActive = true;
    }

    public MenuSection AddSection(MenuSectionId id, string name, int sortOrder)
    {
        MenuSection section = MenuSection.Create(id, this, name, sortOrder);
        _sections.Add(section);
        return section;
    }

    public bool RemoveSection(MenuSectionId id)
    {
        MenuSection? section = _sections.FirstOrDefault(candidate => candidate.Id == id);
        return section is not null && _sections.Remove(section);
    }
}
