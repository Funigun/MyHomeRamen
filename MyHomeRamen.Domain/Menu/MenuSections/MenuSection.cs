using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Domain.Menu.MenuSectionItems;
using MyHomeRamen.Domain.Menu.Offers;

namespace MyHomeRamen.Domain.Menu.MenuSections;

public sealed class MenuSection : AuditableEntity, IEntity<MenuSectionId>
{
    private readonly List<MenuSectionItem> _items = [];

    public MenuSectionId Id { get; private set; }

    public MenuCardId MenuCardId { get; private set; }

    public string Name { get; private set; } = default!;

    public int SortOrder { get; private set; }

    public MenuCard MenuCard { get; private set; } = default!;

    public IReadOnlyCollection<MenuSectionItem> Items => _items.AsReadOnly();

    private MenuSection() { }

    private MenuSection(MenuSectionId id, MenuCard menuCard, string name, int sortOrder)
    {
        Id = id;
        MenuCardId = menuCard.Id;
        MenuCard = menuCard;
        Name = name;
        SortOrder = sortOrder;
    }

    public static MenuSection Create(MenuSectionId id, MenuCard menuCard, string name, int sortOrder)
    {
        MenuSection section = new(id, menuCard, name, sortOrder);
        MenuSectionValidator.ValidateMenuSection(section);
        return section;
    }

    public void Update(string name, int sortOrder)
    {
        Name = name;
        SortOrder = sortOrder;
        MenuSectionValidator.ValidateMenuSection(this);
    }

    public MenuSectionItem AddOffer(MenuSectionItemId id, RestaurantProductOffer offer, int sortOrder)
    {
        MenuSectionItem item = MenuSectionItem.Create(id, this, offer, sortOrder);
        if (_items.Any(existing => existing.RestaurantProductOfferId == item.RestaurantProductOfferId))
        {
            throw MenuSectionItemErrors.DuplicateOffer();
        }

        _items.Add(item);
        return item;
    }

    public bool RemoveOffer(MenuSectionItemId id)
    {
        MenuSectionItem? item = _items.FirstOrDefault(candidate => candidate.Id == id);
        return item is not null && _items.Remove(item);
    }

    public void UpdateOfferSortOrder(MenuSectionItemId id, int sortOrder)
    {
        MenuSectionItem? item = _items.FirstOrDefault(candidate => candidate.Id == id);
        if (item is null)
        {
            throw MenuSectionItemErrors.ItemNotFound();
        }

        item.UpdateSortOrder(sortOrder);
    }
}
