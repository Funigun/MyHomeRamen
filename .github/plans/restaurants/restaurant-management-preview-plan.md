# Restaurant management features

## Create restaurant

Module: Restaurants
Aggregate: Restaurant

Domain logic:
- Create restaurant with name.
Domain events: None

Route: api/restaurants/restaurants
Authorization logic: required permission: Restaurants.Create
Validation logic: name unique within company, name required, max length 200 (as per domain constant).
Handler logic: create new restaurant, return 201 Created with location header pointing to `GET /api/restaurants/restaurants/{restaurantId}`.

## Get restaurant by ID

Module: Restaurants
Aggregate: Restaurant

Domain logic:
- Return restaurant details: id, name, isActive, Address, ContactDetails, BankAccount, and Manager (firstName, lastName, email, phone).
Domain events: None

Route: api/restaurants/restaurants/{restaurantId}
Authorization logic: permission: Restaurant.Manage
Validation logic: validate restaurantId
Handler logic: need to define external api service within Identity module to expose method to get Restaurant Manager details by restaurant ID.

## Get available restaurants

Module: Restaurants
Aggregate: Restaurant

Domain logic:
- Return list items containing id, name, isActive, Address, and ContactDetails.
- Order by created date.
Domain events: None

Route:
Authorization logic: permission: Restaurants.Manage
Validation logic: N/A
Handler logic: Define method in RestaurantQuery to return expected results

## Assign manager

Module: Restaurants
Aggregate: Restaurant

Domain logic:
- Assign manager to restaurant.
- Create `MyHomeRamen.Domain.Identity.CompanyMemberships.CompanyMembership.cs` with `Restaurant Admin` role.
- CompanyMembership: CompanyMemberShipId, CompanyId, RestaurantId, UserId, RoleId
Domain events: None

Route:
Authorization logic: permission: Restaurants.Manage
Validation logic: use same approach as for RegisterCompanyMember command, validate if RestaurantId is valid
Handler logic: the same logic as for RegisterCompanyMember command, but with `Restaurant Admin` role.
