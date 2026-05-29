# Plan: ShoppingCart - ClearBasket

## 1. Problem
User wants endpoint to remove all items from specific basket. Accepts `BasketId` via route, resolves current user from `ICurrentUser`. No ClearBasket feature exists yet. `Basket.Clear()` domain method missing.

## 2. Files to create / modify
| Path | Action | Type | Notes |
|------|--------|------|-------|
| `MyHomeRamen.Domain\ShoppingCart\Baskets\Basket.cs` | Modify | | Add `Clear()` method |
| `MyHomeRamen.Persistance\ShoppingCart\Extensions\BasketDbExtensions.cs` | Modify | | Add `FindByIdAsync` + `GetByIdTracked` |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\ClearBasket\ClearBasketCommand.cs` | Create | command-void | `(Guid BasketId, Guid UserId)` — no request DTO, no response |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\ClearBasket\ClearBasketHandler.cs` | Create | command-void-handler | |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\ClearBasket\ClearBasketValidationPolicy.cs` | Create | validator | 3 DB-backed rules |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\ClearBasket\ClearBasketEndpoint.cs` | Create | endpoint-delete | |
| `MyHomeRamen.IntegrationTests\ShoppingCartModule\Baskets\ClearBasketTests.cs` | Create | integration-test | |

## 3. Domain changes
- `Basket.Clear()` — removes all items from `BasketItems` collection
- Migration needed: no

## 4. Persistence extensions
- `IQueryable<Basket>.FindByIdAsync(BasketId, CancellationToken)` — `AsNoTracking`, returns `Basket?` by id; used by validator for 3-rule check in one query
- `IQueryable<Basket>.GetByIdTracked(BasketId)` — tracked, returns `IQueryable<Basket>` filtered by id; used by handler

## 5. API details
- Endpoint: `DELETE api/shoppingcart/basket/{basketId}/items`
- Auth: `AllowAnonymous`
- Request: `[FromRoute] Guid basketId` + `ICurrentUser.UserId` → Response: `204 No Content`
- Validation rules:
  - Basket with `basketId` exists
  - Basket `UserId` matches `CurrentUser.UserId`
  - Basket `Status == BasketStatus.Active`

## 6. Tests
- Integration:
  - `ClearBasket_ShouldReturnNoContent_ForValidRequest` (happy)
  - `ClearBasket_ShouldReturnNotFound_ForNonExistentBasket`
  - `ClearBasket_ShouldReturnBadRequest_ForBasketNotBelongingToUser`
  - `ClearBasket_ShouldReturnBadRequest_ForInactiveBasket`

## 7. Risks / decisions for human approval
- `Basket.Clear()` removes items from in-memory collection — EF cascade delete must already be configured for `BasketItem`; assuming yes. Verify before implementing.
- No request DTO in `Common.Contracts` — route-only, no body. Validator targets `ClearBasketCommand` directly.

## 8. Out of scope
- Deleting the basket entity itself
- Returning updated basket state
