# Plan: Delete Basket Item – Frontend

## References

- `ShoppingCartApiClient.cs` in `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/ShoppingCart/Common/Services/` – existing typed HTTP client pattern
- `BasketSummaryMenu.razor` in `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/ShoppingCart/Baskets/Components/` – component to update
- `AddItemToBasketResponse.cs` / `GetCurrentBasketSummaryResponse.cs` in `Features/ShoppingCart/Baskets/Responses/` – existing response records
- `BasketItemTableModel.cs` in `Features/ShoppingCart/Baskets/Models/` – UI model used in component
- `MyHomeRamen.BlazorTests/MenuModule/Categories/` – Blazor test pattern reference

## Implementation Plan

### Step 1: Create Frontend Feature Structure

No new folders required. The delete feature touches two existing files:
- `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/ShoppingCart/Common/Services/ShoppingCartApiClient.cs`
- `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/ShoppingCart/Baskets/Components/BasketSummaryMenu.razor`

### Step 2: Create or Update API Communication Services and API Response Model

**Update: `ShoppingCartApiClient.cs`**

Add the `DeleteBasketItemAsync` method:

```csharp
public async Task DeleteBasketItemAsync(Guid basketItemId, CancellationToken ct = default)
{
    using HttpResponseMessage response = await httpClient.DeleteAsync(
        $"/api/shoppingcart/basket/items/{basketItemId}", ct);
    response.EnsureSuccessStatusCode();
}
```

- Follows the same `httpClient.{Verb}Async(...)` + `EnsureSuccessStatusCode()` pattern used by `AddItemToBasketAsync`.
- Returns `Task` (no return value) because the API responds with `204 NoContent` — no body to deserialize.
- URL: `/api/shoppingcart/basket/items/{basketItemId}` — matches the new DELETE endpoint route.

No new response model needed — `204 NoContent` carries no body.

### Step 3: Create or Update Models, DTOs and Mappings

No new models, DTOs, or mappings required. The delete operation sends only a basket item ID (embedded in the URL) and receives no response body. All existing models (`BasketItemTableModel`, `BasketItemResponse`) remain unchanged.

### Step 4: Create or Update Blazor Components and Pages

**Update: `BasketSummaryMenu.razor`**

Replace the stub `HandleRemoveAsync` implementation:

```csharp
// BEFORE (stub):
private Task HandleRemoveAsync(Guid id)
{
    return Task.CompletedTask;
}

// AFTER (implemented):
private async Task HandleRemoveAsync(Guid id)
{
    await ShoppingCartApiClient.DeleteBasketItemAsync(id);
    _items.RemoveAll(item => item.Id == id);
    _total = _items.Sum(i => i.Price * i.Quantity);
}
```

**Why `RemoveAll` instead of reloading via `LoadBasketAsync`:**
- Avoids a redundant network round-trip; the removed item's ID is already known locally.
- `_total` is recalculated in-place from the updated `_items` list.
- `StateHasChanged()` is called implicitly by Blazor's event system after the `EventCallback` completes — no explicit call needed.

No structural changes to the `.razor` markup — `OnRemove="HandleRemoveAsync"` binding in `BasketMenuPanel` already exists.

### Step 5: Blazor Tests

**Create:** `MyHomeRamen.BlazorTests/ShoppingCartModule/Baskets/BasketSummaryMenuTests.cs`

| # | Test Method | Scenario |
|---|-------------|---------|
| 1 | `BasketSummaryMenu_ShouldCallDeleteApi_WhenHandleRemoveAsyncIsInvoked` | Verifies `ShoppingCartApiClient.DeleteBasketItemAsync` is called with the correct ID |
| 2 | `BasketSummaryMenu_ShouldRemoveItemFromList_WhenDeleteSucceeds` | After delete, the item is no longer in `_items`; `_total` is recalculated |
| 3 | `BasketSummaryMenu_ShouldNotUpdateList_WhenDeleteFails` | If API throws, `_items` remains unchanged (exception propagates) |

**Test infrastructure notes:**
- Mock `ShoppingCartApiClient` using the Blazor test framework's substitution mechanism (same approach used in `MenuModule` Blazor tests).
- Render `BasketSummaryMenu` in isolation with pre-populated `_items` via reflection seeding or a test setup helper.
- Use semantic selectors (not CSS class selectors) to trigger `HandleRemoveAsync`.
- Assert on `_items.Count` and `_total` value after the action completes.
