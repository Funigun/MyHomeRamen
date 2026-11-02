namespace MyHomeRamen.Features.Orders.ExternalApi;

public interface IOrdersService
{
    Task<Guid> CreateOrderAsync(CreateOrderFromCheckoutRequest request, CancellationToken cancellationToken);
}

public enum OrderFulfilmentType
{
    DineIn,
    TakeOut,
    Delivery
}

public sealed record CreateOrderFromCheckoutRequest(
    Guid RestaurantId,
    string Currency,
    Guid UserId,
    OrderFulfilmentType Type,
    IReadOnlyList<OrderLineSnapshot> Lines,
    OrderAddressSnapshot? DeliveryAddress);

public sealed record OrderLineSnapshot(
    Guid ProductId,
    Guid RestaurantProductOfferId,
    string Name,
    decimal UnitPrice,
    int Quantity,
    decimal FinalChargedLinePrice,
    IReadOnlyList<OrderIngredientSnapshot> BaseIngredients,
    IReadOnlyList<OrderIngredientSnapshot> CustomIngredients);

public sealed record OrderIngredientSnapshot(Guid IngredientId, string Name, decimal Price);

public sealed record OrderAddressSnapshot(
    string Street,
    string Building,
    string Apartment,
    string City,
    string ZipCode);
