# Plan: ShoppingCart - Delete Basket Item

## 1. Problem
Users need to remove a specific item from their basket. The endpoint targets a `BasketItem` owned by a `Basket` aggregate, identified by route parameters. No equivalent delete-item endpoint exists yet in the `ShoppingCart` module.

## 2. Files to create / modify

| Path | Action | Type | Notes |
|------|--------|------|-------|
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\DeleteBasketItem\DeleteBasketItemCommand.cs` | create | `command-void` | Holds `BasketId` and `BasketItemId` (both `Guid`) |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\DeleteBasketItem\DeleteBasketItemHandler.cs` | create | `command-void-handler` | Loads basket, calls `RemoveItem`, saves |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\DeleteBasketItem\DeleteBasketItemValidator.cs` | create | `validator` | Checks basket exists; checks item belongs to basket |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\DeleteBasketItem\DeleteBasketItemEndpoint.cs` | create | `endpoint-delete` | Two `[FromRoute] Guid` params; `AllowAnonymous`; returns `204 No Content` |
| `MyHomeRamen.Domain\ShoppingCart\Basket\Basket.cs` | modify | | Add `RemoveItem(BasketItemId)` method |
| `MyHomeRamen.Domain\ShoppingCart\Basket\BasketErrors.cs` | modify | | Add `ItemNotFound` error factory |
| `MyHomeRamen.Persistance\ShoppingCart\Extensions\BasketDbExtensions.cs` | modify | | Add `BasketExistsAsync(BasketId)` and `BasketItemExistsAsync(BasketId, BasketItemId)` extension methods |
| `MyHomeRamen.IntegrationTests\ShoppingCartModule\Baskets\DeleteBasketItemTests.cs` | create | `integration-test` | |
| `MyHomeRamen.UnitTests\ShoppingCartModule\Basket\BasketValidationTests.cs` | modify | | Add `RemoveItem` method tests |

## 3. Domain changes
- `Basket.RemoveItem(BasketItemId basketItemId)` — removes the matching child entity; raises no cross-aggregate event
- `BasketErrors.ItemNotFound()` — `DomainException` factory for missing basket item
- Migration needed: **no**

## 4. Persistence extensions
- `BasketExistsAsync(BasketId basketId, CancellationToken ct)` — used by validator to confirm basket exists
- `BasketItemExistsAsync(BasketId basketId, BasketItemId basketItemId, CancellationToken ct)` — used by validator to confirm item belongs to basket

## 5. API details
- Endpoint: `DELETE /api/shoppingcart/baskets/{basketId}/items/{basketItemId}`
- Auth: `AllowAnonymous`
- Request: `[FromRoute] Guid basketId`, `[FromRoute] Guid basketItemId` → Response: `204 No Content`
- Validation rules: basket with `basketId` must exist; basket item with `basketItemId` must belong to that basket — DB checks via persistence extensions; domain behaviour covered by unit tests (§6)

## 6. Tests
- Unit: `RemoveItem_ValidItem` (happy), `RemoveItem_ItemNotFound` (exception)
- Integration: `DeleteBasketItem_ValidIds` (happy), `DeleteBasketItem_BasketNotFound` (not-found), `DeleteBasketItem_ItemNotFound` (not-found)

## 7. Risks / decisions for human approval
- None — straightforward delete; no cross-module side effects required.

## 8. Out of scope
- Recalculating basket totals after item removal (handled by separate domain logic if it already exists)
- Emitting an integration event on item removal
