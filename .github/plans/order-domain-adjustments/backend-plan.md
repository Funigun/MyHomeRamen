# Plan: Orders - Order Domain Adjustments

## 1. Problem
Current `Order` aggregate was built before checkout flow existed. It needs to be updated to consume basket items (products + ingredients), `ShippingDetails`, and `PaymentDetails` snapshots from the basket at order creation. The existing `OrderAddress DeliveryAddress` field must be replaced/evolved to use shipping snapshot. The `Order` must also properly determine its `OrderType` based on shipping details.

## 2. Files to create / modify
| Path | Action | Type | Notes |
|------|--------|------|-------|
| `MyHomeRamen.Domain\Orders\ShippingDetails\OrderShippingDetails.cs` | Create | | Value object snapshot: `PersonalPickup`, `Delivery`, `OrderShippingAddress?` |
| `MyHomeRamen.Domain\Orders\ShippingDetails\OrderShippingAddress.cs` | Create | | Owned value object: `Street`, `Building`, `Apartment`, `City`, `ZipCode` |
| `MyHomeRamen.Domain\Orders\PaymentDetails\OrderPaymentDetails.cs` | Create | | Value object snapshot: `PaymentMethodId`, `PaymentChannelId` |
| `MyHomeRamen.Domain\Orders\Orders\Order.cs` | Modify | | Replace `DeliveryAddress` with `OrderShippingDetails`; add `OrderPaymentDetails`; refactor factory methods to accept basket data; derive `OrderType` from shipping |
| `MyHomeRamen.Domain\Orders\Orders\OrderValidator.cs` | Modify | | Update validation for new fields; remove `DeliveryAddress` rule; add shipping/payment rules |
| `MyHomeRamen.Domain\Orders\Orders\OrderAddress.cs` | Delete | | Replaced by `OrderShippingDetails` + `OrderShippingAddress` |
| `MyHomeRamen.Domain\Orders\Ingredients\Ingredient.cs` | Modify | | Ensure ingredient snapshot fields match basket ingredient (add `OriginalId`, `Description` if missing) |
| `MyHomeRamen.Domain\Orders\Products\Product.cs` | Modify | | Add `Comment` field (from basket item comment); add `ImageUrl` if missing; align with BasketItem snapshot |
| `MyHomeRamen.Persistance\Orders\Configurations\OrderConfiguration.cs` | Modify | | Map `OrderShippingDetails` (owned with `OrderShippingAddress`), `OrderPaymentDetails` (owned); remove `OrderAddress` mapping |
| `MyHomeRamen.Persistance\Orders\Migrations\20260614_UpdateOrderForCheckout` | Create | | Migration |

## 3. Domain changes
- `Order` drops `OrderAddress DeliveryAddress`; gains `OrderShippingDetails ShippingDetails` and `OrderPaymentDetails PaymentDetails`
- `OrderType` derived in factory method: `PersonalPickup = true` → `TakeOut`; `Delivery = true` → `Delivery`; table context passed separately for `DineIn`
- Factory methods signature updated: `Create(OrderId, IEnumerable<Product>, OrderShippingDetails, OrderPaymentDetails)` — existing `CreateDineIn`/`CreateTakeOut`/`CreateDelivery` may be collapsed or retained; `CreateByCustomer` and `CreateByRestaurant` factory methods added
- `Product` snapshot aligned with basket: add `Comment`, ensure `OriginalId` chain matches
- Migration needed: yes — `20260614_UpdateOrderForCheckout`

## 4. Persistence extensions
- None new — extensions will be added in create-order feature plans

## 5. API details
- No new endpoints in this plan — purely domain + persistence layer changes

## 6. Tests
**Unit tests:** `MyHomeRamen.UnitTests\OrdersModule\Orders\OrderValidationTests.cs`
- `CreateByCustomer_ShouldThrow_WhenShippingDetailsNull`
- `CreateByCustomer_ShouldThrow_WhenPaymentDetailsNull`
- `CreateByCustomer_ShouldThrow_WhenNoProducts`
- `CreateByCustomer_ShouldSetTypeToDelivery_WhenDeliveryFlagTrue`
- `CreateByCustomer_ShouldSetTypeToTakeOut_WhenPersonalPickupFlagTrue`

## 7. Risks / decisions for human approval
- `OrderAddress` file deletion — confirm nothing else references it before removing
- Confirm whether existing `CreateDineIn`/`CreateTakeOut`/`CreateDelivery` factory methods are kept or replaced with `CreateByCustomer`/`CreateByRestaurant`
- `OrderType.DineIn` determination: confirm it is set based on restaurant context (no shipping needed) vs shipping flags

## 8. Out of scope
- Order status machine changes
- Payment processing integration
