using FluentValidation;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.ShippingDetails;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetShippingDetails;

public sealed record GetShippingDetailsQuery(BasketId BasketId, UserId UserId) : IQuery<ShippingDetailsResponse>;

public sealed class GetShippingDetailsValidationPolicy : AbstractValidator<GetShippingDetailsQuery>
{
    public GetShippingDetailsValidationPolicy(IShoppingCartDbContext dbContext)
    {
        RuleFor(x => x.BasketId)
            .MustBeAccessibleBasket(
                dbContext,
                query => query.UserId);
    }
}
public sealed record GetShippingDetailsQueryOptions(BasketId BasketId, UserId UserId)
    : DbQueryOptions<Basket, ShippingDetailsDto?>
    (
        new()
        {
            Filter = basket => basket.Id == BasketId && basket.UserId == UserId && basket.Status == BasketStatus.Active,
            Selector = basket => MapToShippingDetailsDto(basket.ShippingDetails)
        }
    )
{
    private static ShippingDetailsDto? MapToShippingDetailsDto(ShippingDetails? shippingDetails)
    {
        if (shippingDetails is null)
        {
            return null;
        }

        ShippingAddressDto? addressDto = MapToShippingAddressDto(shippingDetails.ShippingAddress);
        
        return new ShippingDetailsDto(
            shippingDetails.PersonalPickup,
            shippingDetails.Delivery,
            addressDto);
    }

    private static ShippingAddressDto? MapToShippingAddressDto(ShippingAddress? address)
    {
        return address is null ? null : new ShippingAddressDto(
            address.Street,
            address.Building,
            address.Apartment,
            address.City,
            address.ZipCode);
    }
}

public sealed class GetShippingDetailsHandler(IShoppingCartDbContext dbContext) : IQueryHandler<GetShippingDetailsQuery, ShippingDetailsResponse>
{
    public async Task<ShippingDetailsResponse> Handle(GetShippingDetailsQuery query, CancellationToken cancellationToken)
    {
        ShippingDetailsDto? shippingDetails = await dbContext.Basket.Query()
            .GetShippingDetailsAsync(new GetShippingDetailsQueryOptions(query.BasketId, query.UserId), cancellationToken);

        return shippingDetails is null
            ? throw new InvalidOperationException("Basket was not found.")
            : new ShippingDetailsResponse(shippingDetails.PersonalPickup, shippingDetails.Delivery, shippingDetails.ShippingAddress);
    }
}
