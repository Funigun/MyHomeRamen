using MyHomeRamen.Domain.Common.Order;
using MyHomeRamen.Domain.Orders.Ingredients;
using MyHomeRamen.Domain.Orders.Orders;
using MyHomeRamen.Domain.Orders.Products;
using MyHomeRamen.Domain.Orders.Users;
using MyHomeRamen.Features.Orders.ExternalApi;
using MyHomeRamen.Features.Orders.Features.Abstractions;
using MyHomeRamen.Features.Restaurants.ExternalApi;

namespace MyHomeRamen.Features.Orders.Services;

public sealed class OrdersService(IOrdersDbContext dbContext, IRestaurantService restaurantService) : IOrdersService
{
    public async Task<Guid> CreateOrderAsync(CreateOrderFromCheckoutRequest request, CancellationToken cancellationToken)
    {
        RestaurantInfo? restaurant = await restaurantService.GetRestaurantAsync(request.RestaurantId, cancellationToken);
        if (restaurant is not { IsActive: true } || restaurant.Id != request.RestaurantId)
        {
            throw OrderErrors.RestaurantUnavailable();
        }

        List<Product> products = request.Lines.Select(CreateProductSnapshot).ToList();
        OrderId orderId = new(Guid.CreateVersion7());
        UserId userId = new(request.UserId);
        Order order = request.Type switch
        {
            OrderFulfilmentType.DineIn => Order.CreateDineIn(orderId, userId, request.RestaurantId, request.Currency, products),
            OrderFulfilmentType.TakeOut => Order.CreateTakeOut(orderId, userId, request.RestaurantId, request.Currency, products),
            OrderFulfilmentType.Delivery => Order.CreateDelivery(
                orderId,
                userId,
                request.RestaurantId,
                request.Currency,
                request.DeliveryAddress is null
                    ? throw new ArgumentException("Delivery address is required for delivery orders.", nameof(request))
                    : OrderAddress.Create(
                        request.DeliveryAddress.Street,
                        request.DeliveryAddress.City,
                        request.DeliveryAddress.Building,
                        request.DeliveryAddress.Apartment,
                        request.DeliveryAddress.ZipCode),
                products),
            _ => throw new ArgumentOutOfRangeException(nameof(request.Type), request.Type, "Unsupported order fulfilment type.")
        };

        dbContext.Order.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return order.Id.Value;
    }

    private static Product CreateProductSnapshot(OrderLineSnapshot line)
        => Product.Create(
            new ProductId(Guid.CreateVersion7()),
            new ProductId(line.ProductId),
            line.RestaurantProductOfferId,
            line.Name,
            line.UnitPrice,
            line.Quantity,
            line.FinalChargedLinePrice,
            line.BaseIngredients.Select(CreateIngredientSnapshot).ToList(),
            line.CustomIngredients.Select(CreateIngredientSnapshot).ToList());

    private static Ingredient CreateIngredientSnapshot(OrderIngredientSnapshot ingredient)
        => Ingredient.Create(
            new IngredientId(Guid.CreateVersion7()),
            new IngredientId(ingredient.IngredientId),
            ingredient.Name,
            ingredient.Price);
}
