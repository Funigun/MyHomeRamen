using FluentValidation;
using MyHomeRamen.Domain.Common.Basket;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Menu.ExternalApi;
using MyHomeRamen.Features.Orders.ExternalApi;
using MyHomeRamen.Features.Restaurants.ExternalApi;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.CheckoutBasket;

public sealed record CheckoutBasketCommand(CheckoutBasketRequest Request) : ICommand<CheckoutBasketResponse?>;

public sealed record CheckoutBasketRequest(Guid RestaurantId, string Currency);

public sealed record CheckoutBasketResponse(Guid OrderId, Guid RestaurantId);

public sealed class CheckoutBasketAuthorizationPolicy(ICurrentUser currentUser)
    : IAuthorizationPolicy<CheckoutBasketCommand>
{
    public Task<bool> Authorize(CheckoutBasketCommand request, CancellationToken cancellationToken)
        => Task.FromResult(currentUser.CanManageBasket());
}

public sealed class CheckoutBasketValidator : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketValidator(ICompanyContext companyContext)
    {
        RuleFor(command => command.Request.RestaurantId)
            .NotEmpty()
            .WithMessage("RestaurantId must not be empty.")
            .Must(restaurantId => restaurantId == companyContext.RestaurantId)
            .WithMessage("RestaurantId must match the X-Restaurant-Id header.");

        RuleFor(command => command.Request.Currency)
            .Must(currency => currency is { Length: 3 } && currency.All(char.IsAsciiLetter))
            .WithMessage("Currency must be a three-letter code.");
    }
}

public sealed class CheckoutBasketHandler(
    IShoppingCartDbContext dbContext,
    ICurrentUser currentUser,
    IRestaurantService restaurantService,
    IMenuService menuService,
    IOrdersService ordersService)
    : IRequestHandler<CheckoutBasketCommand, CheckoutBasketResponse?>
{
    public async Task<CheckoutBasketResponse?> Handle(
        CheckoutBasketCommand command,
        CancellationToken cancellationToken)
    {
        CheckoutBasketRequest request = command.Request;
        UserId userId = new(currentUser.UserId);
        Basket? basket = await dbContext.Basket.Load()
            .GetForUserWithCheckoutTrackedAsync(userId, cancellationToken);

        if (basket is null)
        {
            return null;
        }

        if (basket.RestaurantId != request.RestaurantId)
        {
            throw BasketErrors.BasketRestaurantMismatch();
        }

        RestaurantInfo? restaurant = await restaurantService.GetRestaurantAsync(request.RestaurantId, cancellationToken);
        if (restaurant is not { IsActive: true } || restaurant.Id != basket.RestaurantId)
        {
            throw BasketErrors.RestaurantUnavailable();
        }

        if (basket.Items.Count == 0 || basket.ShippingDetails is null)
        {
            throw BasketErrors.BasketNotReadyForCheckout();
        }

        List<OrderLineSnapshot> lines = [];
        foreach (BasketItem basketItem in basket.Items)
        {
            MenuOfferResult? offer = await menuService.GetOfferForBasketAsync(
                basket.RestaurantId,
                basketItem.RestaurantProductOfferId,
                cancellationToken);
            if (offer is null
                || offer.RestaurantId != basket.RestaurantId
                || offer.ProductId != basketItem.Product.OriginalId.Value)
            {
                throw BasketErrors.OfferUnavailable();
            }

            Dictionary<Guid, MenuOfferIngredientResult> availableExtras = offer.AvailableExtras
                .ToDictionary(extra => extra.Id);
            List<OrderIngredientSnapshot> customIngredients = [];
            decimal extraPrice = 0m;
            foreach (Ingredient selectedExtra in basketItem.Product.CustomIngredients)
            {
                if (!availableExtras.TryGetValue(selectedExtra.OriginalId.Value, out MenuOfferIngredientResult? currentExtra))
                {
                    throw BasketErrors.OfferUnavailable();
                }

                extraPrice += currentExtra.Price * selectedExtra.Quantity;
                customIngredients.Add(new OrderIngredientSnapshot(
                    currentExtra.Id,
                    currentExtra.Name,
                    currentExtra.Price));
            }

            lines.Add(new OrderLineSnapshot(
                basketItem.Product.OriginalId.Value,
                offer.OfferId,
                offer.Name,
                offer.Price,
                basketItem.Quantity,
                (offer.Price + extraPrice) * basketItem.Quantity,
                offer.BaseIngredients
                    .Select(ingredient => new OrderIngredientSnapshot(ingredient.Id, ingredient.Name, ingredient.Price))
                    .ToList(),
                customIngredients));
        }

        bool isDelivery = basket.ShippingDetails.Delivery;
        OrderAddressSnapshot? deliveryAddress = isDelivery && basket.ShippingDetails.ShippingAddress is not null
            ? new OrderAddressSnapshot(
                basket.ShippingDetails.ShippingAddress.Street,
                basket.ShippingDetails.ShippingAddress.Building,
                basket.ShippingDetails.ShippingAddress.Apartment,
                basket.ShippingDetails.ShippingAddress.City,
                basket.ShippingDetails.ShippingAddress.ZipCode)
            : null;

        if (isDelivery && deliveryAddress is null)
        {
            throw BasketErrors.BasketNotReadyForCheckout();
        }

        Guid orderId = await ordersService.CreateOrderAsync(
            new CreateOrderFromCheckoutRequest(
                basket.RestaurantId,
                request.Currency,
                userId.Value,
                isDelivery ? OrderFulfilmentType.Delivery : OrderFulfilmentType.TakeOut,
                lines,
                deliveryAddress),
            cancellationToken);

        basket.CheckOut();
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CheckoutBasketResponse(orderId, basket.RestaurantId);
    }
}
