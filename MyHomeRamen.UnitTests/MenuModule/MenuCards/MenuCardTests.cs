using System.Collections.ObjectModel;
using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Domain.Menu.MenuSectionItems;
using MyHomeRamen.Domain.Menu.MenuSections;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.UnitTests.MenuModule.MenuCards;

public sealed class MenuCardTests
{
    [Fact]
    public void Create_ShouldAllowMultipleCards_WhenRestaurantMatches()
    {
        Guid restaurantId = Guid.CreateVersion7();

        MenuCard firstCard = CreateMenuCard(restaurantId, "Lunch");
        MenuCard secondCard = CreateMenuCard(restaurantId, "Dinner");

        Assert.NotEqual(firstCard.Id, secondCard.Id);
        Assert.Equal(restaurantId, firstCard.RestaurantId);
        Assert.Equal(restaurantId, secondCard.RestaurantId);
    }

    [Fact]
    public void UpdateSortOrder_ShouldSetSortOrder_WhenValueIsNonNegative()
    {
        MenuCard card = CreateMenuCard(Guid.CreateVersion7());

        card.UpdateSortOrder(3);

        Assert.Equal(3, card.SortOrder);
    }

    [Fact]
    public void UpdateSortOrder_ShouldThrowDomainException_WhenValueIsNegative()
    {
        MenuCard card = CreateMenuCard(Guid.CreateVersion7());

        DomainException exception = Assert.Throws<DomainException>(() => card.UpdateSortOrder(-1));

        Assert.Equal(MenuCardErrors.SortOrderInvalid().Message, exception.Message);
    }

    [Fact]
    public void Activate_ShouldSetActive_WhenCardIsInactive()
    {
        MenuCard card = CreateMenuCard(Guid.CreateVersion7());
        card.SetActive(false);

        card.Activate();

        Assert.True(card.IsActive);
    }

    [Fact]
    public void Activate_ShouldThrowDomainException_WhenCardIsAlreadyActive()
    {
        MenuCard card = CreateMenuCard(Guid.CreateVersion7());

        DomainException exception = Assert.Throws<DomainException>(() => card.Activate());

        Assert.Equal(MenuCardErrors.AlreadyActive().Message, exception.Message);
    }

    [Fact]
    public void AddOffer_ShouldThrowDomainException_WhenOfferIsAlreadyListed()
    {
        Guid restaurantId = Guid.CreateVersion7();
        MenuCard card = CreateMenuCard(restaurantId);
        MenuSection section = card.AddSection(
            new MenuSectionId(Guid.CreateVersion7()),
            "Popular",
            0);
        RestaurantProductOffer offer = CreateOffer(restaurantId);
        section.AddOffer(new MenuSectionItemId(Guid.CreateVersion7()), offer, 0);

        DomainException exception = Assert.Throws<DomainException>(() =>
            section.AddOffer(new MenuSectionItemId(Guid.CreateVersion7()), offer, 1));

        Assert.Equal(MenuSectionItemErrors.DuplicateOffer().Message, exception.Message);
    }

    [Fact]
    public void AddOffer_ShouldThrowDomainException_WhenOfferBelongsToAnotherRestaurant()
    {
        MenuCard card = CreateMenuCard(Guid.CreateVersion7());
        MenuSection section = card.AddSection(
            new MenuSectionId(Guid.CreateVersion7()),
            "Popular",
            0);

        DomainException exception = Assert.Throws<DomainException>(() =>
            section.AddOffer(
                new MenuSectionItemId(Guid.CreateVersion7()),
                CreateOffer(Guid.CreateVersion7()),
                0));

        Assert.Equal(MenuSectionItemErrors.WrongRestaurant().Message, exception.Message);
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenNameExceedsMaximumLength()
    {
        DomainException exception = Assert.Throws<DomainException>(() =>
            CreateMenuCard(Guid.CreateVersion7(), new string('x', MenuCardConstants.MaxNameLength + 1)));

        Assert.Equal(MenuCardErrors.NameTooLong().Message, exception.Message);
    }

    [Fact]
    public void AddSection_ShouldThrowDomainException_WhenSortOrderIsNegative()
    {
        MenuCard card = CreateMenuCard(Guid.CreateVersion7());

        DomainException exception = Assert.Throws<DomainException>(() =>
            card.AddSection(new MenuSectionId(Guid.CreateVersion7()), "Popular", -1));

        Assert.Equal(MenuSectionErrors.SortOrderInvalid().Message, exception.Message);
    }

    private static MenuCard CreateMenuCard(Guid restaurantId, string name = "Lunch")
        => MenuCard.Create(new MenuCardId(Guid.CreateVersion7()), restaurantId, name);

    private static RestaurantProductOffer CreateOffer(Guid restaurantId)
    {
        Guid companyId = Guid.CreateVersion7();
        Product product = Product.Create(
            new ProductId(Guid.CreateVersion7()),
            companyId,
            "Test product",
            "A valid product description.",
            string.Empty,
            new Collection<Ingredient>(),
            new Collection<Ingredient>(),
            new Collection<Category>());

        return RestaurantProductOffer.Create(
            new RestaurantProductOfferId(Guid.CreateVersion7()),
            restaurantId,
            companyId,
            product,
            10m);
    }
}
