# Plan: Shopping Cart – Delete Basket Item

## 1. Problem
Users need to remove a specific item from their active basket. The basket module already supports adding and querying items, but has no removal capability. Any user (guest or authenticated) may perform this operation.

## 2. Files to create / modify

| Path | Action | Type | Notes |
|------|--------|------|-------|
| `MyHomeRamen.Domain/ShoppingCart/Baskets/Basket.cs` | modify | | Add `RemoveItem(BasketItemId)` — throws `BasketItemNotFound` if absent |
| `MyHomeRamen.Domain/Common/Basket/BasketErrors.cs` | modify | | Add `BasketItemNotFound()` |
| `MyHomeRamen.Persistance/ShoppingCart/Extensions/BasketDbExtensions.cs` | modify | | Add `GetBasketWithItemsTracked(BasketId)` |
| `MyHomeRamen.Persistance/ShoppingCart/Extensions/BasketItemDbExtensions.cs` | modify | | Add `IsInBasket(BasketItemId, BasketId)` — create file if absent |
| `MyHomeRamen.Common.Contracts/ShoppingCart/Baskets/Requests/DeleteBasketItemRequest.cs` | create | request | |
| `MyHomeRamen.Common.Contracts/ShoppingCart/Baskets/Responses/DeleteBasketItemResponse.cs` | create | response | |
| `MyHomeRamen.Api/ShoppingCart/Features/Baskets/DeleteBasketItem/DeleteBasketItemCommand.cs` | create | command-void | |
| `MyHomeRamen.Api/ShoppingCart/Features/Baskets/DeleteBasketItem/DeleteBasketItemEndpoint.cs` | create | endpoint-delete | |
| `MyHomeRamen.Api/ShoppingCart/Features/Baskets/DeleteBasketItem/DeleteBasketItemHandler.cs` | create | command-void-handler | Load basket with items tracked; call `RemoveItem`; save |
| `MyHomeRamen.Api/ShoppingCart/Features/Baskets/DeleteBasketItem/DeleteBasketItemValidator.cs` | create | validator | |
| `MyHomeRamen.UnitTests/ShoppingCartModule/Baskets/BasketBehaviorTests.cs` | modify | | |
| `MyHomeRamen.IntegrationTests/ShoppingCartModule/Baskets/DeleteBasketItemTests.cs` | create | integration-test | |

## 3. Domain changes
- `Basket.RemoveItem(BasketItemId)`
- `BasketErrors.BasketItemNotFound()`
- Migration needed: no

## 4. API details
- Endpoint: `DELETE api/shoppingcart/basket/{basketId}/items/{itemId}`
- Auth: `AllowAnonymous()`
- Request: `[FromRoute] basketId`, `[FromRoute] itemId` → Response: `204 No Content`
- Validation: basket exists + Active; item belongs to basket

## 5. Tests
- Unit: `RemoveItem_WhenItemExists` (happy), `RemoveItem_WhenItemDoesNotExist` (exception)
- Integration: `DeleteBasketItem_ValidRequest` (happy), `_Unauthorized`, `_EmptyBasketId` (validation), `_EmptyItemId` (validation), `_BasketNotFound` (validation), `_ItemNotInBasket` (validation)

## 6. Risks / decisions for human approval
- **Auth**: Existing basket endpoints use `AllowAnonymous()`. Confirm guests should also be able to delete items (plan assumes yes).
- **Route shape**: Two-param route `{basketId}/items/{itemId}` vs. item-only `items/{itemId}` resolving basket from `ICurrentUser`. Confirm preference.
- **Orphan cleanup**: `Product` / `Ingredient` rows in the basket schema are not cleaned up on item removal. Confirm if out of scope.

## 7. Out of scope
- Clear entire basket
- Update item quantity
- Orphan `Product` / `Ingredient` cleanup
