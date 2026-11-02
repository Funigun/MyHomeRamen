using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Menu.MenuSections;
using MyHomeRamen.Domain.Menu.Offers;

namespace MyHomeRamen.Domain.Menu.MenuSectionItems;

public sealed class MenuSectionItem : AuditableEntity, IEntity<MenuSectionItemId>
{
    public MenuSectionItemId Id { get; private set; }

    public MenuSectionId MenuSectionId { get; private set; }

    public RestaurantProductOfferId RestaurantProductOfferId { get; private set; }

    public int SortOrder { get; private set; }

    public MenuSection MenuSection { get; private set; } = default!;

    public RestaurantProductOffer RestaurantProductOffer { get; private set; } = default!;

    private MenuSectionItem() { }

    private MenuSectionItem(
        MenuSectionItemId id,
        MenuSection section,
        RestaurantProductOffer offer,
        int sortOrder)
    {
        Id = id;
        MenuSectionId = section.Id;
        MenuSection = section;
        RestaurantProductOfferId = offer.Id;
        RestaurantProductOffer = offer;
        SortOrder = sortOrder;
    }

    public static MenuSectionItem Create(
        MenuSectionItemId id,
        MenuSection section,
        RestaurantProductOffer offer,
        int sortOrder)
    {
        MenuSectionItem item = new(id, section, offer, sortOrder);
        MenuSectionItemValidator.ValidateMenuSectionItem(item);
        return item;
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        MenuSectionItemValidator.ValidateMenuSectionItem(this);
    }
}
