using MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Checkout.ShippingDetails;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services;

public sealed class ShoppingCartApiClient(HttpClient httpClient)
{
    public async Task<AddItemToBasketResponse> AddItemToBasketAsync(AddItemToBasketRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/shoppingcart/basket/items", request, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AddItemToBasketResponse>(ct)
            ?? throw new InvalidOperationException("Empty response from AddItemToBasket endpoint.");
    }

    public async Task RemoveItem(Guid basketId, Guid itemId, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/shoppingcart/baskets/{basketId}/items/{itemId}", ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task ClearBasket(Guid basketId, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/shoppingcart/baskets/{basketId}", ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<GetCurrentBasketSummaryResponse?> GetCurrentBasketSummaryAsync(CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<GetCurrentBasketSummaryResponse>("/api/shoppingcart/basket/summary", ct);
    }

    public async Task<GetCurrentBasketDetailsResponse?> GetCurrentBasketDetailsAsync(CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<GetCurrentBasketDetailsResponse>("/api/shoppingcart/basket/details", ct);
    }

    public async Task<ShippingDetailsResponse?> GetShippingDetailsAsync(Guid basketId, CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<ShippingDetailsResponse>($"/api/shopping-cart/{basketId}/shipping-details", ct);
    }

    public async Task<bool> UpdateShippingDetailsAsync(Guid basketId, ShippingDetailsModel model, CancellationToken ct = default)
    {
        ShippingAddressDto? addressDto = null;
        if (model.Delivery && model.ShippingAddress is not null)
        {
            addressDto = new ShippingAddressDto(
                model.ShippingAddress.Street,
                model.ShippingAddress.Building,
                model.ShippingAddress.Apartment,
                model.ShippingAddress.City,
                model.ShippingAddress.ZipCode
            );
        }

        UpdateShippingDetailsRequest request = new(model.PersonalPickup, model.Delivery, addressDto);

        using HttpResponseMessage response = await httpClient.PutAsJsonAsync($"/api/shopping-cart/{basketId}/update-shipping-details", request, ct);
        return response.IsSuccessStatusCode;
    }
}
