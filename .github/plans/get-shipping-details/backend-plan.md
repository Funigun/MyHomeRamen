# Plan: ShoppingCart - Get Shipping Details

## 1. Problem
Customer needs to retrieve shipping details for a basket (personal pickup vs delivery + address). No `ShippingDetails` entity exists yet — must be created as a dependent entity with one-to-one relationship to `Basket` using shadow FK/PK managed by EF.

## 2. Files to create / modify
| Path | Action | Type | Notes |
|------|--------|------|-------|
| `MyHomeRamen.Domain\ShoppingCart\ShippingDetails\ShippingDetails.cs` | Create | | Dependent entity: `PersonalPickup`, `Delivery`, `ShippingAddress?`; no explicit ID |
| `MyHomeRamen.Domain\ShoppingCart\ShippingDetails\ShippingAddress.cs` | Create | | Owned value object: `Street`, `Building`, `Apartment`, `City`, `ZipCode` |
| `MyHomeRamen.Domain\ShoppingCart\Baskets\Basket.cs` | Modify | | Add `ShippingDetails? ShippingDetails` nav property (private setter) |
| `MyHomeRamen.Domain\ShoppingCart\Database\IShoppingCartDbContext.cs` | Modify | | Add `DbSet<ShippingDetails>` |
| `MyHomeRamen.Persistance\ShoppingCart\Configurations\ShippingDetailsConfiguration.cs` | Create | | One-to-one with Basket via shadow FK; owns `ShippingAddress` |
| `MyHomeRamen.Persistance\ShoppingCart\Extensions\BasketDbExtensions.cs` | Modify | | Add `GetByIdForUserWithShipping(BasketId, UserId)` |
| `MyHomeRamen.Persistance\ShoppingCart\Migrations\20260614_AddShippingDetailsToBasket` | Create | | Migration for ShippingDetails table |
| `MyHomeRamen.Common.Contracts\ShoppingCart\Baskets\Responses\ShippingDetailsResponse.cs` | Create | response | `bool PersonalPickup, bool Delivery, ShippingAddressResponse? ShippingAddress` |
| `MyHomeRamen.Common.Contracts\ShoppingCart\Baskets\DTOs\ShippingAddressDto.cs` | Create | response | `string Street, Building, Apartment, City, ZipCode` |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\GetShippingDetails\GetShippingDetailsQuery.cs` | Create | query | `record GetShippingDetailsQuery(BasketId BasketId, UserId UserId)` |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\GetShippingDetails\GetShippingDetailsHandler.cs` | Create | query-handler | Returns `ShippingDetailsResponse?` |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\GetShippingDetails\GetShippingDetailsValidationPolicy.cs` | Create | validator | Validates basket exists, active, belongs to user |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\GetShippingDetails\GetShippingDetailsEndpoint.cs` | Create | endpoint-get | |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\GetShippingDetails\Mappings.cs` | Create | | `ShippingDetails → ShippingDetailsResponse` |

## 3. Domain changes
- `ShippingDetails` — dependent entity tracked by EF via shadow PK + shadow FK to Basket; no public ID
- `ShippingAddress` — owned type embedded in `ShippingDetails`
- `Basket` — add `ShippingDetails? ShippingDetails` (private setter)
- Migration needed: yes — `20260614_AddShippingDetailsToBasket`

## 4. Persistence extensions
- `GetByIdForUserWithShipping(BasketId, UserId)` — `AsNoTracking`, filter active basket by id + user, `.Include(b => b.ShippingDetails)`

## 5. API details
**Response:** `ShippingDetailsResponse(bool PersonalPickup, bool Delivery, ShippingAddressDto? ShippingAddress)`
`ShippingAddressDto(string Street, string Building, string Apartment, string City, string ZipCode)`

**Query:** `GetShippingDetailsQuery(BasketId BasketId, UserId UserId)` — BasketId from route param `{id}`, UserId from `ICurrentUser`

**Validator:** use existing basket-existence DB extension; assert basket exists + active + belongs to current user

**Handler:** fetch via `GetByIdForUserWithShipping` → map to response; return `null`

**Endpoint:** `GET api/shopping-cart/{id}/shipping-details` — `AllowAnonymous` — tag `Baskets`

## 6. Tests
**Integration tests:** `MyHomeRamen.IntegrationTests\ShoppingCartModule\Baskets\GetShippingDetailsTests.cs`
- `GetShippingDetails_ShouldReturnOk_ForBasketWithShippingDetails`
- `GetShippingDetails_ShouldReturnNotFound_ForNonExistentBasket`
- `GetShippingDetails_ShouldReturnNotFound_ForBasketOfAnotherUser`

## 7. Risks / decisions for human approval
- `ShippingDetails` has no explicit domain ID — confirm shadow PK approach vs typed `ShippingDetailsId`
- Response when basket exists but `ShippingDetails` not yet set: `200 OK` with null body vs separate empty response shape

## 8. Out of scope
- Shipping fee calculation
- Address validation against external provider
