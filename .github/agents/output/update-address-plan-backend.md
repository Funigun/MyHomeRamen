# Feature Backend Plan — UpdateAddress

- **Date**: 2025-07-17
- **Feature**: UpdateAddress — `PUT /users/me/addresses/{id}`

---

## 1) Domain changes

### Files to modify:
- `MyHomeRamen.Domain/Users/Address.cs` — Add `Update(string street, string building, string apartment, string city, string zipCode)` method that applies new field values via private setters
- `MyHomeRamen.Domain/Users/User.cs` — Add `UpdateAddress(Guid addressId, string street, string building, string apartment, string city, string zipCode, bool isDefault)` method that:
  1. Finds the address by ID in `_addresses` — throws `DomainException` if not found
  2. Calls `address.Update(...)` with field values
  3. If `isDefault == true` and the target is not already default: unset current default (if any different), set target as default
  4. If `isDefault == false` and the target is currently default: unset it (user left with no default)

### Files to create/modify:
- *No new Domain errors required. Object availability is checked in the Validation Policy.*

---

## 2) Persistence changes

No migration required — `IsDefault` column is added by the AddAddress feature migration.

---

## 3) Contracts

No new contract validators needed — reuse the address field validators created in AddAddress feature (`StreetValidator`, `BuildingValidator`, etc.).

---

## 4) API feature — UpdateAddress

### Folder structure:
```
MyHomeRamen.Identity.Api/
└── Features/
    └── Account/
        └── Addresses/
            └── UpdateAddress/
                ├── Models/
                │   ├── UpdateAddressRequest.cs
                │   └── Mappings.cs
                ├── Policies/
                │   └── UpdateAddressValidationPolicy.cs
                ├── UpdateAddressEndpoint.cs
                └── UpdateAddressHandler.cs
```

### Files to create:

#### `UpdateAddressRequest.cs`
- `public sealed record UpdateAddressRequestId(Guid Id);`
- `public sealed record UpdateAddressRequest(Guid Id, string Street, string Building, string Apartment, string City, string ZipCode, bool IsDefault) : IRequest;`

#### `UpdateAddressResponse.cs`
- `public sealed record UpdateAddressResponse(Guid Id);`

#### `Mappings.cs`
- Delete or omit if not needed (the response is just the wrapped ID).

#### `UpdateAddressValidationPolicy.cs`
- `AbstractValidator<UpdateAddressRequest>` implementing `IValidationPolicy<UpdateAddressRequest>`
- Validate `Street`, `Building`, `Apartment`, `City`, `ZipCode` using shared contract validators
- Validate `Id` is not empty
- DB validation: check address exists and belongs to the authenticated user via `IUsersDbContext` (this guarantees the object is available before the domain is called, negating the need for a domain-level AddressNotFound error)

#### `UpdateAddressEndpoint.cs`
- `IEndpoint` with `GroupName = "Account"`
- `MapStandardValidatedPutWithResponse<UpdateAddressRequest, UpdateAddressResponse>("/me/addresses/{id}", HandleAsync)`
- `RequireAuthorization()` (any authenticated user)
- `WithName("UpdateAddressEndpoint")`
- `HandleAsync` signature: `([FromRoute] UpdateAddressRequestId id, [FromBody] UpdateAddressRequest request, [FromServices] IRequestHandler<UpdateAddressRequest, UpdateAddressResponse> handler, CancellationToken cancellationToken)`
- Returns: `await handler.Handle(request with { Id = id.Id }, cancellationToken);`

#### `UpdateAddressHandler.cs`
- `public sealed class UpdateAddressHandler(IUsersDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<UpdateAddressRequest, UpdateAddressResponse>`
- Load `User` by `KeycloakUserId` with `.Include(u => u.Addresses)`
- Call `user.UpdateAddress(request.Id, request.Street, request.Building, request.Apartment, request.City, request.ZipCode, request.IsDefault)`
- `SaveChangesAsync()`
- Return `new UpdateAddressResponse(request.Id)`

---

## 5) Endpoint handler flow

1. Extract `KeycloakUserId` from `ICurrentUser.Id`
2. Load `User` with addresses
3. Call `user.UpdateAddress(...)` (domain handles field update + default swap)
4. `SaveChangesAsync()`
5. Return `200 OK` with `UpdateAddressResponse` (wrapped ID)

---

## 7) Create unit tests

### Files to create:
- `MyHomeRamen.UnitTests/UsersModule/Addresses/AddressUpdateTests.cs`
  - `Update_Should_UpdateFields_When_DataIsValid`
  - `Update_Should_NotChangeIsDefault_When_UpdateCalled`

- `MyHomeRamen.UnitTests/UsersModule/Users/UserUpdateAddressTests.cs`
  - `UpdateAddress_Should_UpdateAddress_When_Valid`
  - `UpdateAddress_Should_SetAsDefault_AndUnsetPreviousDefault`
  - `UpdateAddress_Should_UnsetDefault_WhenIsDefaultFalse_OnCurrentDefault`
  - `UpdateAddress_Should_NotChangeDefault_WhenIsDefaultFalse_OnNonDefault`

---

## 8) Create integration tests

### Files to create/modify:
- `MyHomeRamen.IntegrationTests/IdentityModule/Common/Data/DataGenerator.cs` — Add valid/invalid `UpdateAddressRequest` generation
- `MyHomeRamen.IntegrationTests/IdentityModule/Addresses/UpdateAddressTests.cs`
  - `UpdateAddress_ShouldReturn200_WithWrappedId`
  - `UpdateAddress_ShouldReturn200_AndSwapDefault_WhenIsDefaultTrue`
  - `UpdateAddress_ShouldReturn404_WhenAddressNotFound`
  - `UpdateAddress_ShouldReturn401_WhenUnauthenticated`
  - `UpdateAddress_ShouldReturn400_WhenPayloadInvalid`

---

## 9) Architecture tests

Not in scope.

---

## 10) System tests

Not in scope.
