# Plan: ShoppingCart - Update Payment Details

## 1. Problem
Customer needs to save payment method + channel on the basket. Requires cross-module validation: `PaymentMethodId` and `PaymentChannelId` must exist and be active in the Payments module. Depends on `PaymentDetails` domain entity from `get-payment-details` plan. Payments module must expose `IPaymentService` (same pattern as `IMenuService`).

## 2. Files to create / modify
| Path | Action | Type | Notes |
|------|--------|------|-------|
| `MyHomeRamen.Common.Contracts\Payments\IPaymentService.cs` | Create | | Interface: `ValidatePaymentSelectionAsync(Guid methodId, Guid channelId, CancellationToken)` |
| `MyHomeRamen.Api\Payments\Services\PaymentService.cs` | Modify | | Implement `IPaymentService`; replace `Temp.cs` content |
| `MyHomeRamen.Api\Payments\DependencyInjection.cs` | Modify | | Register `IPaymentService → PaymentService` |
| `MyHomeRamen.Domain\ShoppingCart\Baskets\Basket.cs` | Modify | | Add `UpdatePaymentDetails(PaymentDetails)` method |
| `MyHomeRamen.Persistance\ShoppingCart\Extensions\BasketDbExtensions.cs` | Modify | | Add `GetByIdForUserWithPaymentTracked(BasketId, UserId)` |
| `MyHomeRamen.Common.Contracts\ShoppingCart\Baskets\Requests\UpdatePaymentDetailsRequest.cs` | Create | request | `string PaymentMethodId, string PaymentChannelId` |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\UpdatePaymentDetails\UpdatePaymentDetailsCommand.cs` | Create | command-void | `record UpdatePaymentDetailsCommand(BasketId BasketId, UserId UserId, UpdatePaymentDetailsRequest Request)` |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\UpdatePaymentDetails\UpdatePaymentDetailsHandler.cs` | Create | command-void-handler | |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\UpdatePaymentDetails\UpdatePaymentDetailsValidationPolicy.cs` | Create | validator | Basket check + IPaymentService validation |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\UpdatePaymentDetails\UpdatePaymentDetailsEndpoint.cs` | Create | endpoint-put | |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\UpdatePaymentDetails\Mappings.cs` | Create | | `UpdatePaymentDetailsRequest → PaymentDetails domain object` |
| `MyHomeRamen.Api\ShoppingCart\DependencyInjection.cs` | Modify | | Register `IPaymentService` reference (from Payments module DI) |

## 3. Domain changes
- `Basket.UpdatePaymentDetails(PaymentDetails details)` — replaces existing or sets new `PaymentDetails`; validates basket active
- Migration needed: no — schema introduced in `get-payment-details` plan

## 4. Persistence extensions
- `GetByIdForUserWithPaymentTracked(BasketId, UserId)` — tracked (no `AsNoTracking`), filter active basket by id + user, `.Include(b => b.PaymentDetails)`

## 5. API details
**Payments module integration:**
`IPaymentService` in `MyHomeRamen.Common.Contracts\Payments\IPaymentService.cs`:
```
Task<bool> ValidatePaymentSelectionAsync(Guid methodId, Guid channelId, CancellationToken ct)
```
`PaymentService` in `MyHomeRamen.Api\Payments\Services\PaymentService.cs` — query `IPaymentsDbContext` to check method + channel both exist and `IsActive = true` and channel belongs to method

**Request:** `UpdatePaymentDetailsRequest(string PaymentMethodId, string PaymentChannelId)`

**Command:** `UpdatePaymentDetailsCommand(BasketId, UserId, UpdatePaymentDetailsRequest)` — BasketId from route `{id}`

**Validator rules:**
- Basket must exist, active, belong to user (DB extension)
- `PaymentMethodId` and `PaymentChannelId` must parse as valid Guids
- `IPaymentService.ValidatePaymentSelectionAsync` must return `true`

**Handler:** fetch via `GetByIdForUserWithPaymentTracked` → map to `PaymentDetails` domain object → `Basket.UpdatePaymentDetails(...)` → `SaveChangesAsync`

**Endpoint:** `PUT api/shopping-cart/{id}/update-payment-details` — `AllowAnonymous` — returns `200 OK` — tag `Baskets`

## 6. Tests
**Integration tests:** `MyHomeRamen.IntegrationTests\ShoppingCartModule\Baskets\UpdatePaymentDetailsTests.cs`
- `UpdatePaymentDetails_ShouldReturnOk_ForValidPaymentSelection`
- `UpdatePaymentDetails_ShouldReturnBadRequest_ForInvalidMethodId`
- `UpdatePaymentDetails_ShouldReturnBadRequest_ForInactiveChannel`
- `UpdatePaymentDetails_ShouldReturnNotFound_ForNonExistentBasket`

## 7. Risks / decisions for human approval
- `IPaymentService` registered in ShoppingCart DI — confirm cross-module service registration pattern (ShoppingCart DI injects IPaymentService from Payments module)
- Payments module `PaymentService` currently has `Temp.cs` placeholder — confirm file replace approach

## 8. Out of scope
- Actual payment processing / charging
