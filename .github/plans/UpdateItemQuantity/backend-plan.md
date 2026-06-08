# Plan: ShoppingCart - Update Basket Item Quantity

## 1. Problem
User wants to update quantity of an existing item in an active basket. Basket and item must belong to current user. Price must be recalculated on quantity change. No existing `UpdateItemQuantity` slice exists; `DeleteBasketItem` is the closest reference pattern.

## 2. Files to create / modify
| Path | Action | Type | Notes |
|------|--------|------|-------|
| `MyHomeRamen.Common.Contracts\ShoppingCart\BasketItems\Requests\UpdateItemQuantityRequest.cs` | Create | request | `int Quantity` |
| `MyHomeRamen.Domain\ShoppingCart\Baskets\Basket.cs` | Modify | | Add `UpdateBasketItemQuantity(BasketItemId, int quantity)` |
| `MyHomeRamen.Domain\ShoppingCart\BasketItems\BasketItem.cs` | Modify | | Add `UpdateQuantity(int quantity)` with price recalculation |
| `MyHomeRamen.Persistance\ShoppingCart\Extensions\BasketDbExtensions.cs` | Modify | | Add `GetByIdForUserTrackedWithProducts` — tracked, includes Items → Product, filters basketId/userId/active |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\UpdateItemQuantity\UpdateItemQuantityCommand.cs` | Create | command-void | Wraps `BasketId`, `BasketItemId`, `UpdateItemQuantityRequest` |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\UpdateItemQuantity\UpdateItemQuantityHandler.cs` | Create | command-void-handler | |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\UpdateItemQuantity\UpdateItemQuantityValidationPolicy.cs` | Create | validator | |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\UpdateItemQuantity\UpdateItemQuantityEndpoint.cs` | Create | endpoint-put | |
| `MyHomeRamen.UnitTests\ShoppingCartModule\Basket\BasketUpdateItemQuantityTests.cs` | Create | unit-test | |
| `MyHomeRamen.UnitTests\ShoppingCartModule\BasketItem\BasketItemUpdateQuantityTests.cs` | Create | unit-test | |
| `MyHomeRamen.IntegrationTests\ShoppingCartModule\BasketItems\UpdateItemQuantityTests.cs` | Create | integration-test | |

## 3. Domain changes
- `Basket.UpdateBasketItemQuantity(BasketItemId basketItemId, int quantity)` — finds item by id, guards item not found, delegates to `item.UpdateQuantity(quantity)`
- `BasketItem.UpdateQuantity(int quantity)` — sets `Quantity`, recalculates `Price = Product.TotalPrice * Quantity`; guards quantity ≥ `BasketConstants.MinQuantity`
- `BasketErrors.ItemNotFound` (if not already present)
- Migration needed: **no**

## 4. Persistence extensions
- `GetByIdForUserTrackedWithProducts(BasketId, UserId)` — tracked, `.Include(b => b.Items).ThenInclude(i => i.Product)`, filters by basketId/userId/active

## 5. API details
- Endpoint: `PUT api/shoppingcart/baskets/{basketId}/items/{basketItemId}`
- Auth: `AllowAnonymous`
- Request: `[FromRoute] Guid basketId`, `[FromRoute] Guid basketItemId`, `[FromBody] UpdateItemQuantityRequest` → Response: `204 No Content`
- Validation rules:
  - `ItemExistsQuery(userId, basketItemId, basketId, ct)` must return `true` — covers basket active + belongs to user + has item
  - `Quantity` between `BasketConstants.MinQuantity` and `BasketConstants.MaxQuantity`

## 6. Tests
- Unit:
  - `UpdateBasketItemQuantity_ShouldUpdatePrice_WhenItemExists`
  - `UpdateBasketItemQuantity_ShouldThrow_WhenItemNotFound`
  - `UpdateQuantity_ShouldRecalculatePrice_WhenQuantityValid`
  - `UpdateQuantity_ShouldThrow_WhenQuantityBelowMin`
- Integration:
  - `UpdateItemQuantity_ShouldReturnNoContent_ForValidRequest`
  - `UpdateItemQuantity_ShouldReturnNotFound_ForMissingBasketOrItem`
  - `UpdateItemQuantity_ShouldReturnBadRequest_ForInvalidQuantity`
  - `UpdateItemQuantity_ShouldReturnUnauthorized_ForWrongUser`

## 7. Risks / decisions for human approval
- `BasketItem.UpdateQuantity` needs access to `Product.TotalPrice` for recalculation — confirm `BasketItem` already has a navigation to `Product` (indicated by `GetByIdForUserTrackedWithProducts` include path).
- `ItemExistsQuery` checks basket+user+item in one shot; single `NotFound` error returned without distinguishing basket-not-found vs item-not-found. Acceptable?

## 8. Out of scope
- Updating item comment
- Bulk quantity updates
