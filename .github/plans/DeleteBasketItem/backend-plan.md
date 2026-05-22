# Plan: ShoppingCart - Delete Basket Item

## 1. Problem
Users need to remove a specific item from their basket. The endpoint accepts `basketId` and `basketItemId` from the route and is accessible anonymously. No equivalent endpoint exists yet.

## 2. Files to create / modify
| Path | Action | Type | Notes |
|------|--------|------|-------|
| `MyHomeRamen.Api\ShoppingCart\Features\BasketItems\DeleteBasketItem\DeleteBasketItemCommand.cs` | create | `command-void` | No request body — `BasketId` and `BasketItemId` are route params only |
| `MyHomeRamen.Api\ShoppingCart\Features\BasketItems\DeleteBasketItem\DeleteBasketItemEndpoint.cs` | create | `endpoint-delete` | Route: `/baskets/{basketId}/items/{basketItemId}`; `AllowAnonymous` |
| `MyHomeRamen.Api\ShoppingCart\Features\BasketItems\DeleteBasketItem\DeleteBasketItemHandler.cs` | create | `command-void-handler` | |
| `MyHomeRamen.Api\ShoppingCart\Features\BasketItems\DeleteBasketItem\DeleteBasketItemValidator.cs` | create | `validator` | Existence checks via DB extensions |
| `MyHomeRamen.Persistance\ShoppingCart\Extensions\BasketItemDbExtensions.cs` | modify | | Add existence check scoped to a basket |
| `MyHomeRamen.IntegrationTests\ShoppingCartModule\BasketItems\DeleteBasketItemTests.cs` | create | `integration-test` | |

## 3. Domain changes
- `Basket.RemoveItem(BasketItemId itemId)` — domain method to encapsulate item removal
- Migration needed: no

## 4. API details
- Endpoint: `DELETE /baskets/{basketId}/items/{basketItemId}`
- Auth: `AllowAnonymous`
- Request: `[FromRoute] Guid basketId`, `[FromRoute] Guid basketItemId` → Response: `204 No Content`
- Validation rules: basket with `basketId` must exist; basket item with `basketItemId` must exist and belong to that basket

## 5. Tests
- Unit: `RemoveItem_Happy` (happy)
- Unit: `RemoveItem_ItemNotFound` (exception)
- Integration: `DeleteBasketItem_Happy` (happy)
- Integration: `DeleteBasketItem_BasketNotFound` (not-found)
- Integration: `DeleteBasketItem_BasketItemNotFound` (not-found)
- Integration: `DeleteBasketItem_ItemNotBelongingToBasket` (validation)

## 6. Risks / decisions for human approval
- Anonymous access means any caller knowing both IDs can delete the item — confirm this is intentional.
- Confirm whether `BasketItemGroup.cs` route group already exists; if not, it must be created.

## 7. Out of scope
- Recalculating basket totals after item removal
- Soft-delete vs hard-delete strategy for basket items
