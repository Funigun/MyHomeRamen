# Plan: Orders - Create Order by Customer

## 1. Problem
Customer completes checkout by submitting a BasketId. The API must fetch the basket (with items, shipping, and payment details) from the ShoppingCart module, create an Order domain aggregate, persist it, and raise `OrderCreatedByCustomerEvent`. ShoppingCart module must expose `IShoppingCartService` for basket checkout data retrieval (same pattern as `IMenuService`).

## 2. Files to create / modify
| Path | Action | Type | Notes |
|------|--------|------|-------|
| `MyHomeRamen.Common.Contracts\ShoppingCart\IShoppingCartService.cs` | Create | | Interface: `GetBasketForCheckoutAsync(Guid basketId, Guid userId, CancellationToken)` returning `BasketCheckoutResult?` |
| `MyHomeRamen.Common.Contracts\ShoppingCart\BasketCheckoutResult.cs` | Create | | Result record: items, shipping details, payment details snapshots |
| `MyHomeRamen.Api\ShoppingCart\Services\ShoppingCartService.cs` | Create | | Implements `IShoppingCartService`; queries `IShoppingCartDbContext` for active basket + items + shipping + payment |
| `MyHomeRamen.Api\ShoppingCart\DependencyInjection.cs` | Modify | | Register `IShoppingCartService → ShoppingCartService` |
| `MyHomeRamen.Domain\Orders\Events\OrderCreatedByCustomerEvent.cs` | Create | | `record OrderCreatedByCustomerEvent(OrderId OrderId) : IDomainEvent` |
| `MyHomeRamen.Domain\Orders\Orders\Order.cs` | Modify | | Add `CreateByCustomer(OrderId, IEnumerable<Product>, OrderShippingDetails, OrderPaymentDetails)` factory; raises `OrderCreatedByCustomerEvent` |
| `MyHomeRamen.Persistance\Orders\Extensions\OrderDbExtensions.cs` | Create | | (new file) — add as needed for future queries; stub for now |
| `MyHomeRamen.Common.Contracts\Orders\Orders\Requests\CreateOrderByCustomerRequest.cs` | Create | request | `record CreateOrderByCustomerRequest(Guid BasketId)` |
| `MyHomeRamen.Common.Contracts\Orders\Orders\Responses\CreateOrderResponse.cs` | Create | response | `record CreateOrderResponse(Guid OrderId)` |
| `MyHomeRamen.Api\Orders\Features\Orders\CreateOrderByCustomer\CreateOrderByCustomerCommand.cs` | Create | command | `record CreateOrderByCustomerCommand(UserId UserId, CreateOrderByCustomerRequest Request)` |
| `MyHomeRamen.Api\Orders\Features\Orders\CreateOrderByCustomer\CreateOrderByCustomerHandler.cs` | Create | command-handler | Returns `Guid` (OrderId) |
| `MyHomeRamen.Api\Orders\Features\Orders\CreateOrderByCustomer\CreateOrderByCustomerValidationPolicy.cs` | Create | validator | Basket exists, active, belongs to user; shipping + payment details present |
| `MyHomeRamen.Api\Orders\Features\Orders\CreateOrderByCustomer\CreateOrderByCustomerEndpoint.cs` | Create | endpoint-post | Returns `201 Created` with `Location` header |
| `MyHomeRamen.Api\Orders\Features\Orders\CreateOrderByCustomer\Mappings.cs` | Create | | `BasketCheckoutResult → Order domain objects` |
| `MyHomeRamen.Api\Orders\Features\Orders\OrdersGroup.cs` | Create | | Endpoint group definition |
| `MyHomeRamen.Api\Orders\DependencyInjection.cs` | Modify | | Register handler, validator, inject `IShoppingCartService` |

## 3. Domain changes
- `Order.CreateByCustomer(OrderId, products, shippingDetails, paymentDetails)` — sets type from shipping, raises `OrderCreatedByCustomerEvent`
- `OrderCreatedByCustomerEvent` carries `OrderId`
- Migration needed: no — schema changes handled in `order-domain-adjustments` plan

## 4. Persistence extensions
- `OrderDbExtensions.cs` stub created; no specific query needed for creation

## 5. API details
**IShoppingCartService** (`MyHomeRamen.Common.Contracts\ShoppingCart\IShoppingCartService.cs`):
```
Task<BasketCheckoutResult?> GetBasketForCheckoutAsync(Guid basketId, Guid userId, CancellationToken ct)
```
`BasketCheckoutResult` holds: `BasketId`, `IReadOnlyList<BasketItemResult> Items`, `ShippingDetailsResult ShippingDetails`, `PaymentDetailsResult PaymentDetails`

**Request:** `CreateOrderByCustomerRequest(Guid BasketId)`

**Command:** `CreateOrderByCustomerCommand(UserId UserId, CreateOrderByCustomerRequest Request)` — UserId from `ICurrentUser`

**Validator rules:**
- `BasketId` must be a valid non-empty Guid
- Basket must exist, be active, belong to current user (via `IShoppingCartService`)
- `ShippingDetails` must be set on basket
- `PaymentDetails` must be set on basket

**Handler:**
1. Call `IShoppingCartService.GetBasketForCheckoutAsync`
2. Map basket items → `Product` domain objects (with ingredients)
3. Map shipping → `OrderShippingDetails`
4. Map payment → `OrderPaymentDetails`
5. `Order.CreateByCustomer(newOrderId, products, shippingDetails, paymentDetails)`
6. Persist → `SaveChangesAsync`
7. Return `OrderId`

**Endpoint:** `POST api/orders/by-customer` — `AllowAnonymous` — `201 Created` — tag `Orders`

## 6. Tests
**Integration tests:** `MyHomeRamen.IntegrationTests\OrdersModule\Orders\CreateOrderByCustomerTests.cs`
- `CreateOrderByCustomer_ShouldReturnCreated_ForValidBasket`
- `CreateOrderByCustomer_ShouldReturnBadRequest_WhenShippingDetailsMissing`
- `CreateOrderByCustomer_ShouldReturnBadRequest_WhenPaymentDetailsMissing`
- `CreateOrderByCustomer_ShouldReturnNotFound_ForNonExistentBasket`

## 7. Risks / decisions for human approval
- `IShoppingCartService` in `Common.Contracts\ShoppingCart` — confirm namespace matches `IMenuService` pattern
- Basket status after order creation: should basket be transitioned to `PendingOrder`? Not in scope here — confirm
- `OrderCreatedByCustomerEvent` payload: only `OrderId` or full basket snapshot?

## 8. Out of scope
- Basket status transition to `PendingOrder`/`CheckedOut`
- Payment initiation
- Order confirmation notification
