using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Domain.Menu.Offers;

public sealed class RestaurantProductOffer : AuditableEntity, IEntity<RestaurantProductOfferId>
{
    public RestaurantProductOfferId Id { get; private set; }

    public Guid RestaurantId { get; private set; }

    public Guid CompanyId { get; private set; }

    public ProductId ProductId { get; private set; }

    public decimal Price { get; private set; }

    public RestaurantProductOfferStatus Status { get; private set; }

    public Product Product { get; private set; } = default!;

    private RestaurantProductOffer() { }

    private RestaurantProductOffer(RestaurantProductOfferId id, Guid restaurantId, Guid companyId, Product product)
    {
        Id = id;
        RestaurantId = restaurantId;
        CompanyId = companyId;
        ProductId = product.Id;
        Product = product;
    }

    public static RestaurantProductOffer Create(RestaurantProductOfferId id, Guid restaurantId, Guid companyId, Product product, decimal price)
    {
        RestaurantProductOffer offer = new(id, restaurantId, companyId, product)
        {
            Price = price,
            Status = RestaurantProductOfferStatus.Active
        };

        RestaurantProductOfferValidator.Validate(offer);
        return offer;
    }

    public void UpdatePrice(decimal price)
    {
        Price = price;
        RestaurantProductOfferValidator.Validate(this);
    }

    public void Retire() => Status = RestaurantProductOfferStatus.Retired;
}
