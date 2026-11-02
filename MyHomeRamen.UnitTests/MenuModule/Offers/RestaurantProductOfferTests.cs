using System.Collections.ObjectModel;
using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.UnitTests.MenuModule.Offers;

public sealed class RestaurantProductOfferTests
{
    [Fact]
    public void Create_ShouldAllowDifferentPricesForSharedProduct_WhenRestaurantsDiffer()
    {
        Guid companyId = Guid.CreateVersion7();
        Product product = CreateProduct(companyId);
        Guid firstRestaurantId = Guid.CreateVersion7();
        Guid secondRestaurantId = Guid.CreateVersion7();

        RestaurantProductOffer firstOffer = RestaurantProductOffer.Create(
            new RestaurantProductOfferId(Guid.CreateVersion7()),
            firstRestaurantId,
            companyId,
            product,
            12.50m);
        RestaurantProductOffer secondOffer = RestaurantProductOffer.Create(
            new RestaurantProductOfferId(Guid.CreateVersion7()),
            secondRestaurantId,
            companyId,
            product,
            14.00m);

        Assert.Equal(product.Id, firstOffer.ProductId);
        Assert.Equal(12.50m, firstOffer.Price);
        Assert.Equal(14.00m, secondOffer.Price);
        Assert.Equal(RestaurantProductOfferStatus.Active, firstOffer.Status);
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenCompanyDoesNotMatchProduct()
    {
        Guid productCompanyId = Guid.CreateVersion7();
        Product product = CreateProduct(productCompanyId);

        DomainException exception = Assert.Throws<DomainException>(() =>
            RestaurantProductOffer.Create(
                new RestaurantProductOfferId(Guid.CreateVersion7()),
                Guid.CreateVersion7(),
                Guid.CreateVersion7(),
                product,
                12.50m));

        Assert.Equal(RestaurantProductOfferErrors.CompanyMismatch().Message, exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_ShouldThrowDomainException_WhenPriceIsNotPositive(decimal price)
    {
        Guid companyId = Guid.CreateVersion7();

        DomainException exception = Assert.Throws<DomainException>(() =>
            RestaurantProductOffer.Create(
                new RestaurantProductOfferId(Guid.CreateVersion7()),
                Guid.CreateVersion7(),
                companyId,
                CreateProduct(companyId),
                price));

        Assert.Equal(RestaurantProductOfferErrors.PriceMustBePositive().Message, exception.Message);
    }

    private static Product CreateProduct(Guid companyId)
        => Product.Create(
            new ProductId(Guid.CreateVersion7()),
            companyId,
            "Valid product name",
            "A valid product description for testing.",
            string.Empty,
            new Collection<Ingredient>(),
            new Collection<Ingredient>(),
            new Collection<Category>());
}
