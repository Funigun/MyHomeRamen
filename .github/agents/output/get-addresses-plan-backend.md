# Feature Backend Plan — GetAddresses

- **Date**: 2025-07-17
- **Feature**: GetAddresses — `GET /users/me/addresses`

---

## 1) Domain changes

No domain entity changes required — `User` already has `Addresses` navigation property and `Address` entity has all needed fields (including `IsDefault` added by AddAddress feature).

---

## 2) Persistence changes

No migration required — `Addresses` table already exists.

### Files to create (optional):
- `MyHomeRamen.Persistance/Users/Extensions/AddressDbExtensions.cs` — Query extension: `ForUser(string keycloakUserId)` returning `IQueryable<Address>` filtered by user

---

## 3) Contracts

No new contract validators — this is a read-only endpoint with no request body.

---

## 4) API feature — GetAddresses

### Folder structure:
```
MyHomeRamen.Identity.Api/
└── Features/
    └── Account/
        └── Addresses/
            └── GetAddresses/
                ├── Models/
                │   ├── GetAddressesResponse.cs
                │   └── Mappings.cs
                ├── GetAddressesEndpoint.cs
                └── GetAddressesHandler.cs
```

### Files to create:

#### `GetAddressesResponse.cs`
- `public sealed record GetAddressesResponse(IEnumerable<AddressDto> Addresses);`
- `public sealed record AddressDto(Guid Id, string Street, string Building, string Apartment, string City, string ZipCode, bool IsDefault);`

#### `Mappings.cs`
- Create a new `ToResponse()` extension method mapping `Address` to `AddressDto`
#### `GetAddressesEndpoint.cs`
- `IEndpoint` with `GroupName = "Account"`
- `MapStandardGet<GetAddressesResponse>("/me/addresses", Handler)`
- `RequireAuthorization()` (any authenticated user)
- `WithName("GetAddressesEndpoint")`

#### `GetAddressesHandler.cs`
- `public sealed class GetAddressesHandler(IUsersDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<..., GetAddressesResponse>` (or direct endpoint handler)
- Query `Addresses` table with `AsNoTracking()`, filtered by user's `KeycloakUserId`
- Map to `AddressDto` via `Mappings.ToResponse()`
- Return `200 OK` with `GetAddressesResponse`

---

## 5) Endpoint handler flow

1. Extract `KeycloakUserId` from `ICurrentUser.Id`
2. Query addresses for the user with `AsNoTracking()`
3. Map to `AddressDto` list using the new mapping
4. Return `200 OK` with `GetAddressesResponse`

---

## 7) Create unit tests

### Files to create:
- `MyHomeRamen.UnitTests/UsersModule/Addresses/GetAddressesHandlerTests.cs` (if handler is extracted)
  - `Handle_ShouldReturnAddresses_ForAuthenticatedUser`
  - `Handle_ShouldReturnEmptyList_WhenNoAddresses`

Note: For a simple read-only handler, unit tests may be minimal. Integration tests provide more value.

---

## 8) Create integration tests

### Files to create/modify:
- `MyHomeRamen.IntegrationTests/IdentityModule/Addresses/GetAddressesTests.cs`
  - `GetAddresses_ShouldReturn200_WithAddressList`
  - `GetAddresses_ShouldReturn401_WhenUnauthenticated`
  - `GetAddresses_ShouldReturnEmptyList_WhenUserHasNoAddresses`

---

## 9) Architecture tests

Not in scope.

---

## 10) System tests

Not in scope.
