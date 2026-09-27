# Plan: Restaurants - Restaurant Management

## 1. Problem
Restaurants module needs restaurant creation, restaurant detail and list queries, and manager assignment. Manager assignment also requires Identity company-membership support for the `Restaurant Admin` role.

## 2. Files to create / modify
| Action | Module | Aggregate | Feature Name | Endpoint Kind | Route |
|--------|--------|-----------|--------------|---------------|-------|
| Modify | Restaurants | Restaurant | Create restaurant | Command | `api/restaurants/restaurants` |
| Create | Restaurants | Restaurant | Get restaurant by ID | Query | `api/restaurants/restaurants/{restaurantId}` |
| Create | Restaurants | Restaurant | Get available restaurants | Query |  |
| Create | Restaurants | Restaurant | Assign manager | Command |  |
| Create | Identity | CompanyMembership | Restaurant manager membership | Command |  |
| Modify | Restaurants | Restaurant | Restaurant persistence and API wiring |  |  |
| Modify | Identity | CompanyMembership | Company membership persistence and API wiring |  |  |

## 3. Migration required

true

CompanyMembership and its `Restaurant Admin` role relationship require an Identity migration. Existing Restaurant persistence requires no new Restaurant migration unless implementation reveals a schema change.

## 4. Create restaurant

### 4.1 Domain changes
- Extend existing Restaurant aggregate within Restaurants module with name validation and creation behavior.
- No domain events.

### 4.2 Persistance
- Extend existing Restaurants module context, Restaurant repository, query, specification, configuration, and ID converter as required.
- Add company-scoped name uniqueness query used by validation.
- Do not add a Restaurant migration for existing schema.

### 4.3 API details
Request: restaurant name.
Response: created restaurant identifier and location for restaurant details.
Command/Query: `CreateRestaurantCommand`.
Command/Query handler: create Restaurant, add through Restaurants context, save changes, return created response.
Validation policy: required name, maximum length 200, unique name within company.
Authorization policy: `Restaurants.Create`.
Endpoint: `POST api/restaurants/restaurants`; return `201 Created` with location for `GET api/restaurants/restaurants/{restaurantId}`.

### 4.4 Unit Tests
- Restaurant creation succeeds for valid name.
- Restaurant creation rejects missing name.
- Restaurant creation rejects name exceeding maximum length.
- Create validator rejects duplicate name within company.

### 4.5 Integration Tests
- `CreateRestaurant_ShouldReturnCreated_ForValidRequest`.
- Verify location header points to restaurant detail endpoint.
- Verify authorization rejects missing `Restaurants.Create`.
- Verify duplicate company name and invalid name return validation failure.

## 4. Get restaurant by ID

### 4.1 Domain changes
- Expose restaurant detail projection containing ID, name, active state, address, contact details, bank account, and manager summary.
- No domain events.

### 4.2 Persistance
- Add Restaurant query method loading the restaurant detail projection and required manager reference data.
- Keep read query no-tracking and return not-found when restaurant ID does not exist.

### 4.3 API details
Request: validated `restaurantId`.
Response: restaurant ID, name, active state, address, contact details, bank account, and manager first name, last name, email, and phone.
Command/Query: `GetRestaurantByIdQuery`.
Command/Query handler: query Restaurant details and obtain manager details through the Identity external API service.
Validation policy: validate Restaurant ID and restaurant existence according to query policy.
Authorization policy: `Restaurant.Manage`.
Endpoint: `GET api/restaurants/restaurants/{restaurantId}`.

### 4.4 Unit Tests
- Query handler maps restaurant detail projection and manager details.
- Query handler returns not-found for unknown restaurant ID.
- Query validation rejects invalid restaurant ID.

### 4.5 Integration Tests
- `GetRestaurantById_ShouldReturnDetails_ForExistingRestaurant`.
- Verify address, contact details, bank account, and manager fields.
- Verify unknown restaurant and invalid ID responses.
- Verify authorization rejects requests without `Restaurant.Manage`.

## 4. Get available restaurants

### 4.1 Domain changes
- Expose list projection containing ID, name, active state, address, and contact details.
- Order results by creation date.
- No domain events.

### 4.2 Persistance
- Add Restaurant query method returning the expected list projection ordered by creation date.
- Use no-tracking read access.

### 4.3 API details
Request:
Response: restaurant list items containing ID, name, active state, address, and contact details.
Command/Query: `GetAvailableRestaurantsQuery`.
Command/Query handler: query Restaurant list and map results to response.
Validation policy:
Authorization policy: `Restaurants.Manage`.
Endpoint: route to be defined.

### 4.4 Unit Tests
- Query handler maps all requested list fields.
- Query results preserve creation-date ordering.

### 4.5 Integration Tests
- `GetAvailableRestaurants_ShouldReturnOrderedItems_ForExistingRestaurants`.
- Verify response excludes fields not part of list projection.
- Verify authorization rejects requests without `Restaurants.Manage`.

## 4. Assign manager

### 4.1 Domain changes
- Add manager assignment behavior to Restaurant aggregate.
- Add Identity `CompanyMembership` entity with CompanyMembershipId, CompanyId, RestaurantId, UserId, and RoleId.
- Add `Restaurant Admin` role support for restaurant membership.
- Validate Restaurant ID using the same rules as RegisterCompanyMember.
- No domain events.

### 4.2 Persistance
- Add Restaurant assignment specification/query for validating and loading the target restaurant.
- Add Identity CompanyMembership configuration, repository/query/specification integration, and migration named `YYYYMMDD_AddRestaurantAdminCompanyMembership`.
- Reuse RegisterCompanyMember persistence behavior where applicable.

### 4.3 API details
Request: restaurant ID and manager user ID.
Response: assigned manager membership or command success response.
Command/Query: `AssignRestaurantManagerCommand`.
Command/Query handler: validate Restaurant, create/register company membership for the manager with `Restaurant Admin` role, and save changes using existing RegisterCompanyMember logic.
Validation policy: validate Restaurant ID using RegisterCompanyMember rules; validate manager and membership inputs.
Authorization policy: `Restaurants.Manage`.
Endpoint: route to be defined.

### 4.4 Unit Tests
- Restaurant accepts valid manager assignment.
- Assignment rejects invalid restaurant.
- CompanyMembership creates Restaurant Admin membership with expected company, restaurant, user, and role identifiers.
- Assignment validation matches RegisterCompanyMember behavior.

### 4.5 Integration Tests
- `AssignRestaurantManager_ShouldAssignManager_ForValidRequest`.
- Verify Restaurant Admin membership is persisted.
- Verify invalid restaurant and invalid manager inputs return validation failure.
- Verify authorization rejects requests without `Restaurants.Manage`.
