# Checkout Backend Plan

## 1. Get Shipping Details

**Purpose:** Load basket shipping details.
**Module:** `ShoppingCart`
**Domain**: Create ShippingDetails value object, extend `Basket` with navigation property to `ShippingDetails` (one-to-one)
**Persistance**: Basket to ShippingDetails is one-to-one with navigation property to ShippingDetails, ShippingDetails without navigation or FK Id property (shadow property will be used for FK by EF)
**API contract:**
```
GET /api/shopping-cart/{id}/shipping-details

ShippingDetailsDto
{
    bool PersonalPickup,
    bool Delivery,
    ShippingAddressDto? ShippingAddress
}

ShippingAddressDto 
{
   string Street
   string Building 
   string Apartment
   string City
   string ZipCode
}

AuthPolicy: AllowAnonymous

Validation Rules:
- basket must exist, be active and belong to user - use existing extension method
```

## 2. Update Shipping Details

**Purpose:** Persist shipping option + address on basket.
**Module:** `ShoppingCart`
**Domain**: extend `Basket` with method to update ShippingDetails.
**Persistance**: Extension method to get active basket by Id and user id with ShippingDetails included.
**API contract:**
```
PUT /api/shopping-cart/{id}/update-shipping-details
Body = ShippingDetailsForUpdateDto

ShippingDetailsForUpdateDto
{
    bool PersonalPickup,
    bool Delivery,
    ShippingAddressForUpdateDto? ShippingAddress
}

ShippingAddressForUpdateDto 
{
   string Street
   string Building 
   string Apartment
   string City
   string ZipCode
}

AuthPolicy: AllowAnonymous

Validation Rules:
- basket must exist, be active and belong to user - use existing method
- if PersonalPickup is false, Delivery must be true and ShippingAddress must be provided
- if Delivery is false, PersonalPickup must be true and ShippingAddress must be null
- PersonalPickup and Delivery cannot both be false

```
---

# 3. Get Payment Details

**Purpose:** Load basket payment details.
**Module:** `ShoppingCart`
**Domain**: Create PaymentDetails value object, extend `Basket` with navigation property to `PaymentDetails` (one-to-one)
**Persistance**: Basket to PaymentDetails is one-to-one with navigation property to PaymentDetails, PaymentDetails without navigation or FK Id property (shadow property will be used for FK by EF)
**API contract:**
```
GET /api/shopping-cart/{id}/payment-details

PaymentDetailsDto
{
    string PaymentMethodId,
    string PaymentChannelId
}

AuthPolicy: AllowAnonymous

Validation Rules:
- basket must exist, be active and belong to user - use existing extension method
```

# 4. Update Payment Details
**Purpose:** Persist payment method + channel on basket.
**Module:** `ShoppingCart` and `Payments`
**Domain**: extend `Basket` with method to update PaymentDetails.
**Persistance**: Extension method to get active basket by Id and user id with PaymentDetails included.
**API contract:**
```
PUT /api/shopping-cart/{id}/update-payment-details
Body = PaymentDetailsForUpdateDto

PaymentDetailsForUpdateDto
{
    string PaymentMethodId,
    string PaymentChannelId
}

AuthPolicy: AllowAnonymous

Validation Rules:
- basket must exist, be active and belong to user - use existing extension method
- PaymentMethodId and PaymentChannelId must exist and be active - verified through Payments module API
```

**Modules Integration**:
- `Payments` module must expose `PaymentService` service to validate PaymentMethodId and PaymentChannelId existence and active status -(the same pattern as `MenuService` from Menu module)

# 5. Order domain adjustments
**Purpose:** Update Order domain to handle changed basket and payment domains.
**Module:** `Orders`
**Domain**: Update Order aggregate to consume BasketItems, ShippingDetails and PaymentDetails from Basket when creating order. Adjust Order creation logic to handle new data and validation rules.
No API contract - only domain and db configuration updates

# 6.1. Create Order by Customer
**Purpose:** Create order from confirmed basket.
**Module:** `Orders`
**Domain**: Add OrderCreatedByCustomerEvent domain with OrderId, BasketDto, ShippingDetailsDto, PaymentDetailsDto. 
**Api contract:**
```
POST /api/orders/by-customer

OrderForCreationDto
{
    Guid BasketId
}

AuthPolicy: AllowAnonymous
```

# 6.2. Create Order by Restaurant
**Purpose:** Create order by restaurant - client id will be table id.
**Module:** `Orders`
**Domain**: Add OrderCreatedInRestaurantEvent domain with OrderId, BasketDto, ShippingDetailsDto, PaymentDetailsDto.
**Api contract:**
```
POST /api/orders/by-restaurant
OrderForCreationInRestaurantDto
{
    Guid BasketId
    Guid TableId
}
```

# 7. Accept Order
**Purpose:** Accept order by restaurant and trigger order processing workflow.
To be confirmed.

# 8. Get Order status
**Purpose:** Load order status history from customer perspective.
To be confirmed, but most probably SSE feature will be used.

# 9. Get new orders
**Purpose:** Load new orders for restaurant for acceptance.
To be confirmed.

# 10. Get orders to prepare
**Purpose:** Load accepted orders for restaurant for preparation (kitchen view).
To be confirmed.

# 11. Mark order as ready
**Purpose:** Mark order as ready by the kitchen and trigger proper event.
To be confirmed, probably events: OrderReadyForPickupEvent, OrderReadyForDeliveryEvent, OrderReadyForDineInEvent will be used.

# 12. Mark order as paid
**Purpose:** Mark order as paid by restaurant and trigger proper event.
To be confirmed.