# Plan: Orders - Create Order by Restaurant

## 1. Problem
Restaurant staff creates an order on behalf of a dine-in customer at a table. The basket is identified by `BasketId`; the table by `TableId`. `OrderType` is always `DineIn`. Requires same ShoppingCart integration as `create-order-by-customer`. Depends on `IShoppingCartService` introduced in that plan.

## 2. Files to create / modify
| Path | Action | Type | Notes |
|------|--------|------|-------|
| `MyHomeRamen.Domain\Orders\Events\OrderCreatedInRestaurantEvent.cs` | Create | | `record OrderCreatedInRestaurantEvent(OrderId OrderId) : IDomainEvent` |
| `MyHomeRamen.Domain\Orders\Orders\Order.cs` | Modify | | Add `CreateByRestaurant(OrderId, IEnumerable<Product>, OrderPaymentDetails, Guid tableId)` factory; type = `DineIn`; raises `OrderCreatedInRestaurantEvent` |
| `MyHomeRamen.Common.Contracts\Orders\Orders\Requests\CreateOrderByRestaurantRequest.cs` | Create | request | `record CreateOrderByRestaurantRequest(Guid BasketId, Guid TableId)` |
| `MyHomeRamen.Api\Orders\Features\Orders\CreateOrderByRestaurant\CreateOrderByRestaurantCommand.cs` | Create | command | `record CreateOrderByRestaurantCommand(UserId UserId, CreateOrderByRestaurantRequest Request)` |
| `MyHomeRamen.Api\Orders\Features\Orders\CreateOrderByRestaurant\CreateOrderByRestaurantHandler.cs` | Create | command-handler | Returns `Guid` (OrderId) |
| `MyHomeRamen.Api\Orders\Features\Orders\CreateOrderByRestaurant\CreateOrderByRestaurantValidationPolicy.cs` | Create | validator | Basket exists + active + payment details present; TableId valid non-empty Guid |
| `MyHomeRamen.Api\Orders\Features\Orders\CreateOrderByRestaurant\CreateOrderByRestaurantEndpoint.cs` | Create | endpoint-post | Returns `201 Created` with `Location` header |
| `MyHomeRamen.Api\Orders\Features\Orders\CreateOrderByRestaurant\Mappings.cs` | Create | | `BasketCheckoutResult → Order domain objects` (reuse mappings from CreateOrderByCustomer where possible) |
| `MyHomeRamen.Api\Orders\DependencyInjection.cs` | Modify | | Register handler, validator |

## 3. Domain changes
- `Order.CreateByRestaurant(OrderId, products, paymentDetails, tableId)` — type is always `DineIn`; `TableId` stored as `ClientId` on Order; raises `OrderCreatedInRestaurantEvent`
- `OrderCreatedInRestaurantEvent` carries `OrderId`
- `Order` may need `TableId` / `ClientId` property — add `Guid ClientId` (private setter)
- Migration needed: yes — `20260614_AddClientIdToOrder` (adds `ClientId` column to Orders table) — or include in `order-domain-adjustments` migration if run together

## 4. Persistence extensions
- None new — creation only

## 5. API details
**Request:** `CreateOrderByRestaurantRequest(Guid BasketId, Guid TableId)`

**Command:** `CreateOrderByRestaurantCommand(UserId UserId, CreateOrderByRestaurantRequest Request)` — UserId from `ICurrentUser`

**Validator rules:**
- `BasketId` and `TableId` must be valid non-empty Guids
- Basket must exist, be active (via `IShoppingCartService`)
- `PaymentDetails` must be set on basket

**Handler:**
1. Call `IShoppingCartService.GetBasketForCheckoutAsync`
2. Map basket items → `Product` domain objects
3. Map payment → `OrderPaymentDetails`
4. `Order.CreateByRestaurant(newOrderId, products, paymentDetails, request.TableId)`
5. Persist → `SaveChangesAsync`
6. Return `OrderId`

**Endpoint:** `POST api/orders/by-restaurant` — `AllowAnonymous` — `201 Created` — tag `Orders`

**Note:** No `ShippingDetails` validation — DineIn orders don't require shipping details on basket

## 6. Tests
**Integration tests:** `MyHomeRamen.IntegrationTests\OrdersModule\Orders\CreateOrderByRestaurantTests.cs`
- `CreateOrderByRestaurant_ShouldReturnCreated_ForValidBasketAndTable`
- `CreateOrderByRestaurant_ShouldReturnBadRequest_WhenPaymentDetailsMissing`
- `CreateOrderByRestaurant_ShouldReturnBadRequest_ForEmptyTableId`
- `CreateOrderByRestaurant_ShouldReturnNotFound_ForNonExistentBasket`

## 7. Risks / decisions for human approval
- `TableId` stored as `ClientId` on Order — confirm field name and whether it should be a typed `TableId` or plain `Guid`
- Auth policy: should `by-restaurant` require an employee/admin role rather than `AllowAnonymous`?
- Shipping details: confirm DineIn orders explicitly do NOT require shipping details on basket

## 8. Out of scope
- Table availability / reservation validation
- Basket status transition after order creation
