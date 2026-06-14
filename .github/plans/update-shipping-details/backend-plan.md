# Plan: ShoppingCart - Update Shipping Details

## 1. Problem
Customer needs to save their shipping choice (personal pickup or delivery + address) on the basket. Depends on `ShippingDetails` domain entity introduced in the `get-shipping-details` plan. `Basket` needs a mutation method to set/replace shipping details.

## 2. Files to create / modify
| Path | Action | Type | Notes |
|------|--------|------|-------|
| `MyHomeRamen.Domain\ShoppingCart\Baskets\Basket.cs` | Modify | | Add `UpdateShippingDetails(ShippingDetails)` method |
| `MyHomeRamen.Persistance\ShoppingCart\Extensions\BasketDbExtensions.cs` | Modify | | Add `GetByIdForUserWithShippingTracked(BasketId, UserId)` — tracked, includes ShippingDetails |
| `MyHomeRamen.Common.Contracts\ShoppingCart\Baskets\Requests\UpdateShippingDetailsRequest.cs` | Create | request | `bool PersonalPickup, bool Delivery, ShippingAddressDto? ShippingAddress` |
| `MyHomeRamen.Common.Contracts\ShoppingCart\Baskets\DTOs\ShippingAddressDto.cs` | Create | request | `string Street, Building, Apartment, City, ZipCode` |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\UpdateShippingDetails\UpdateShippingDetailsCommand.cs` | Create | command-void | `record UpdateShippingDetailsCommand(BasketId BasketId, UserId UserId, UpdateShippingDetailsRequest Request)` |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\UpdateShippingDetails\UpdateShippingDetailsHandler.cs` | Create | command-void-handler | |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\UpdateShippingDetails\UpdateShippingDetailsValidationPolicy.cs` | Create | validator | Basket + shipping rules |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\UpdateShippingDetails\UpdateShippingDetailsEndpoint.cs` | Create | endpoint-put | |
| `MyHomeRamen.Api\ShoppingCart\Features\Baskets\UpdateShippingDetails\Mappings.cs` | Create | | `UpdateShippingDetailsRequest → ShippingDetails domain object` |

## 3. Domain changes
- `Basket.UpdateShippingDetails(ShippingDetails details)` — replaces existing or sets new `ShippingDetails`; validates basket is active
- Migration needed: no — schema introduced in `get-shipping-details` plan

## 4. Persistence extensions
- `GetByIdForUserWithShippingTracked(BasketId, UserId)` — tracked (no `AsNoTracking`), filter active basket by id + user, `.Include(b => b.ShippingDetails)`

## 5. API details
**Request:** `UpdateShippingDetailsRequest(bool PersonalPickup, bool Delivery, ShippingAddressDto? ShippingAddress)`
`ShippingAddressDto(string Street, string Building, string Apartment, string City, string ZipCode)`

**Command:** `UpdateShippingDetailsCommand(BasketId BasketId, UserId UserId, UpdateShippingDetailsRequest Request)` — BasketId from route `{id}`, body bound to request

**Validator rules:**
- Basket must exist, active, belong to user (DB extension)
- `PersonalPickup` and `Delivery` cannot both be `false`
- If `Delivery = true`: `ShippingAddress` must be provided and all address fields non-empty
- If `PersonalPickup = true` and `Delivery = false`: `ShippingAddress` must be `null`

**Handler:** fetch via `GetByIdForUserWithShippingTracked` → map request to `ShippingDetails` domain object → `Basket.UpdateShippingDetails(...)` → `SaveChangesAsync`

**Endpoint:** `PUT api/shopping-cart/{id}/update-shipping-details` — `AllowAnonymous` — returns `200 OK` — tag `Baskets`

## 6. Tests
**Integration tests:** `MyHomeRamen.IntegrationTests\ShoppingCartModule\Baskets\UpdateShippingDetailsTests.cs`
- `UpdateShippingDetails_ShouldReturnOk_ForValidDeliveryRequest`
- `UpdateShippingDetails_ShouldReturnOk_ForPersonalPickup`
- `UpdateShippingDetails_ShouldReturnBadRequest_WhenBothFlagsAreFalse`
- `UpdateShippingDetails_ShouldReturnBadRequest_WhenDeliveryWithoutAddress`
- `UpdateShippingDetails_ShouldReturnNotFound_ForNonExistentBasket`

## 7. Risks / decisions for human approval
- None — straightforward mutation

## 8. Out of scope
- Shipping fee recalculation on update
