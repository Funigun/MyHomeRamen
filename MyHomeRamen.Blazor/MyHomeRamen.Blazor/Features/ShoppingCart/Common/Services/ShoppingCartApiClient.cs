using MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Requests;
using MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Responses;

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

    public async Task<GetCurrentBasketSummaryResponse?> GetCurrentBasketSummaryAsync(CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<GetCurrentBasketSummaryResponse>("/api/shoppingcart/baskets", ct);
    }
}
