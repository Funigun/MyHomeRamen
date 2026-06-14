# Plan: ShoppingCart - Get Payment Details

## 1. Problem
Customer needs to retrieve payment details (method + channel) saved on a basket. No `PaymentDetails` entity exists yet — must be created as a dependent entity with one-to-one relationship to `Basket` using shadow FK/PK managed by EF (same pattern as `ShippingDetails`).

## 2. Files to create / modify
| Path | Action | Type | Notes |
|------|--------|------|-------|
| `MyHomeRamen.Domain\ShoppingCart\PaymentDetails\PaymentDetails.cs` | Create | | Dependent entity: `PaymentMethodId`, `PaymentChannelId`; no explicit ID |
| `MyHomeRamen.Domain\ShoppingCart\Baskets\Basket.cs` | Modify | | Add `PaymentDetails? PaymentDetails` nav property (private setter) |
| `MyHomeRamen.Domain\ShoppingCart\Database\IShoppingCartDbContext.cs` | Modify | | Add `DbSet<PaymentDetails>` |
| `MyHomeRamen.Persistance\ShoppingCart\Configurations\PaymentDetailsConfiguration.cs` | Create | | One-to-one with Basket via shadow FK |
| `MyHomeRamen.Persistance\ShoppingCart\Extensions\BasketDbExtensions.cs` | Modify | | Add `GetByIdForUserWithPayment(BasketId, UserId)` |
| `MyHomeRamen.Persistance\ShoppingCart\Migrations\20260614_AddPaymentDetailsToBasket` | Create | | Migration for PaymentDetails table |
| `MyHomeRamen.Common.Contracts\ShoppingCart\Baskets\Responses\PaymentDetailsResponse.cs` | Create | response | `string PaymentMethodId, string PaymentChannelId` |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\GetPaymentDetails\GetPaymentDetailsQuery.cs` | Create | query | `record GetPaymentDetailsQuery(BasketId BasketId, UserId UserId)` |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\GetPaymentDetails\GetPaymentDetailsHandler.cs` | Create | query-handler | Returns `PaymentDetailsResponse?` |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\GetPaymentDetails\GetPaymentDetailsValidationPolicy.cs` | Create | validator | Validates basket exists, active, belongs to user |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\GetPaymentDetails\GetPaymentDetailsEndpoint.cs` | Create | endpoint-get | |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\GetPaymentDetails\Mappings.cs` | Create | | `PaymentDetails → PaymentDetailsResponse` |

## 3. Domain changes
- `PaymentDetails` — dependent entity tracked by EF via shadow PK + shadow FK to Basket; holds `PaymentMethodId` (string) and `PaymentChannelId` (string)
- `Basket` — add `PaymentDetails? PaymentDetails` (private setter)
- Migration needed: yes — `20260614_AddPaymentDetailsToBasket`

## 4. Persistence extensions
- `GetByIdForUserWithPayment(BasketId, UserId)` — `AsNoTracking`, filter active basket by id + user, `.Include(b => b.PaymentDetails)`

## 5. API details
**Response:** `PaymentDetailsResponse(string PaymentMethodId, string PaymentChannelId)`

**Query:** `GetPaymentDetailsQuery(BasketId BasketId, UserId UserId)` — BasketId from route `{id}`, UserId from `ICurrentUser`

**Validator:** use existing basket-existence DB extension; assert basket exists + active + belongs to current user

**Handler:** fetch via `GetByIdForUserWithPayment` → map to response;

**Endpoint:** `GET api/shopping-cart/{id}/payment-details` — `AllowAnonymous` — tag `Baskets`

## 6. Tests
**Integration tests:** `MyHomeRamen.IntegrationTests\ShoppingCartModule\Baskets\GetPaymentDetailsTests.cs`
- `GetPaymentDetails_ShouldReturnOk_ForBasketWithPaymentDetails`
- `GetPaymentDetails_ShouldReturnNotFound_ForNonExistentBasket`
- `GetPaymentDetails_ShouldReturnNotFound_ForBasketOfAnotherUser`

## 7. Risks / decisions for human approval
- `PaymentMethodId` / `PaymentChannelId` stored as strings (Guid strings) — confirm vs typed IDs on `PaymentDetails` entity

## 8. Out of scope
- Payment method display name/image retrieval
