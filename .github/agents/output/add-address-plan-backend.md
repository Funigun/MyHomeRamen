# Feature Backend Plan — AddAddress

- **Date**: 2025-07-17
- **Feature**: AddAddress — `POST /users/me/addresses`

---

## 1) Domain changes

### Files to modify:
- `MyHomeRamen.Domain/Users/Address.cs` — Add `IsDefault` property (bool, private set), `SetAsDefault()` and `UnsetDefault()` domain methods
- `MyHomeRamen.Domain/Users/User.cs` — Add `AddAddress(Address address)` method that:
  1. Enforces max-5 addresses rule (throws `DomainException` if `_addresses.Count >= 5`)
  2. If `address.IsDefault == true`, iterates `_addresses` and calls `UnsetDefault()` on the current default (if any)
  3. If `address.IsDefault == false` and `_addresses.Count == 0`, automatically sets the new address as default
  4. Adds the address to `_addresses`

### Files to create:
- `MyHomeRamen.Domain/Common/Address/AddressConstants.cs` — Constants: `MaxAddressesPerUser = 5`, `MaxStreetLength`, `MaxBuildingLength`, `MaxApartmentLength`, `MaxCityLength`, `MaxZipCodeLength`
- `MyHomeRamen.Domain/Common/Address/AddressErrors.cs` — Error factories: `MaxAddressesReached()`, field validation errors as needed

### Update Address.Create:
- Add `bool isDefault` parameter to `Address.Create(...)` factory method
- Set `IsDefault` in the factory

---

## 2) Persistence changes

### Files to modify:
- `MyHomeRamen.Persistance/Users/UsersDbContext.cs` — Verify `Address` entity configuration includes `IsDefault` column

### Files to create:
- `MyHomeRamen.Persistance/Users/Configurations/AddressConfiguration.cs` — `IEntityTypeConfiguration<Address>` configuring `IsDefault` column (bool, default `false`)
- `MyHomeRamen.Persistance/Users/Migrations/{timestamp}_AddIsDefaultToAddress.cs` — EF Core migration adding `IsDefault` bit column to `Addresses` table in `identity` schema

### Migration command:
```
dotnet ef migrations add AddIsDefaultToAddress --project MyHomeRamen.Persistance --startup-project MyHomeRamen.Identity.Api --context UsersDbContext --output-dir Users/Migrations
```

---

## 3) Contracts

### Files to create:
- `MyHomeRamen.Common.Contracts/Account/Address/StreetValidator.cs` — `AbstractValidator<string>` with `NotEmpty`, `MaximumLength(AddressConstants.MaxStreetLength)`
- `MyHomeRamen.Common.Contracts/Account/Address/BuildingValidator.cs` — `AbstractValidator<string>` with `NotEmpty`, `MaximumLength`
- `MyHomeRamen.Common.Contracts/Account/Address/ApartmentValidator.cs` — `AbstractValidator<string>` (optional field — may be empty), `MaximumLength`
- `MyHomeRamen.Common.Contracts/Account/Address/CityValidator.cs` — `AbstractValidator<string>` with `NotEmpty`, `MaximumLength`
- `MyHomeRamen.Common.Contracts/Account/Address/ZipCodeValidator.cs` — `AbstractValidator<string>` with `NotEmpty`, `MaximumLength`

Reference: `MyHomeRamen.Common.Contracts/Account/AccountValidationExtensions.cs`

---

## 4) API feature — AddAddress

### Folder structure:
```
MyHomeRamen.Identity.Api/
└── Features/
    └── Account/
        └── Addresses/
            └── AddAddress/
                ├── Models/
                │   ├── AddAddressRequest.cs
                │   ├── AddAddressResponse.cs   ← reusable AddressResponse
                │   └── Mappings.cs
                ├── Policies/
                │   └── AddAddressValidationPolicy.cs
                ├── AddAddressEndpoint.cs
                └── AddAddressHandler.cs
```

### Files to create:

#### `AddAddressRequest.cs`
- `public sealed record AddAddressRequest(string Street, string Building, string Apartment, string City, string ZipCode, bool IsDefault) : IRequest;`

#### `AddAddressResponse.cs`
- `public sealed record AddAddressResponse(Guid Id);`

#### `Mappings.cs`
- `extension(AddAddressRequest)` → `ToAddress()` mapping to `Address.Create(...)`
- `extension(Address)` → `ToResponse()` mapping to `AddressResponse`

#### `AddAddressValidationPolicy.cs`
- `AbstractValidator<AddAddressRequest>` implementing `IValidationPolicy<AddAddressRequest>`
- Rules: validate `Street`, `Building`, `Apartment`, `City`, `ZipCode` using the shared contract validators, validate Max Addresses constraint
- Note: use `CreateCategoryValidator` as example, create proper extension for DbContext

#### `AddAddressEndpoint.cs`
- `IEndpoint` with `GroupName = "Account"`
- `MapStandardValidatedPost<AddAddressRequest, AddAddressResponse>("/me/addresses", HandleAsync)`
- `RequireAuthorization()` (any authenticated user)
- `WithName("AddAddressEndpoint")`
- The `HandleAsync` method should get the ID from the handler, wrap it in `AddAddressResponse`, and return `Results.Created($"/api/account/me/addresses/{id}", response)`

#### `AddAddressHandler.cs`
- `public sealed class AddAddressHandler(IUsersDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<AddAddressRequest, Guid>`
- Load `User` by `KeycloakUserId` from `currentUser.Id` with `.Include(u => u.Addresses)`
- Map request to `Address` via `Mappings.ToAddress()`
- Call `user.AddAddress(address)`
- `SaveChangesAsync()`
- Return `address.Id`

---

## 5) Endpoint handler flow

1. Extract `KeycloakUserId` from `ICurrentUser.Id`
2. Load `User` with addresses from `IUsersDbContext`
3. Map `AddAddressRequest` → `Address` via `Mappings`
4. Call `user.AddAddress(address)` (domain validates max-5, handles default swap)
5. `SaveChangesAsync()`
6. Return `201 Created` with `AddAddressResponse` (wrapping the ID) and the route location

---

## 7) Create unit tests

### Files to create:
- `MyHomeRamen.UnitTests/UsersModule/Addresses/AddressValidationTests.cs`
  - `Create_Should_CreateAddress_When_DataIsValid`
  - `Create_Should_SetIsDefault_When_IsDefaultTrue`
  - `Create_Should_NotSetIsDefault_When_IsDefaultFalse`

- `MyHomeRamen.UnitTests/UsersModule/Users/UserAddAddressTests.cs`
  - `AddAddress_Should_AddAddress_When_UnderLimit`
  - `AddAddress_Should_ThrowDomainException_When_MaxAddressesReached`
  - `AddAddress_Should_SetNewAddressAsDefault_AndUnsetPreviousDefault`
  - `AddAddress_Should_AllowNonDefaultAddress_WhenDefaultAlreadyExists`
  - `AddAddress_Should_AutoSetDefault_WhenFirstAddress_AndIsDefaultFalse`

### Pattern:
- Use private helper `CreateUser()` with defaults
- Use private helper `CreateAddress()` with defaults
- `Assert.Throws<DomainException>` for validation failures
- Assert `Address.IsDefault` and count changes

---

## 8) Create integration tests

### Files to create:
- `MyHomeRamen.IntegrationTests/IdentityModule/Common/Data/DataGenerator.cs` — Generate valid `AddAddressRequest` and invalid request theory data
- `MyHomeRamen.IntegrationTests/IdentityModule/Common/Data/DataSeeder.cs` — Seed test user with addresses for limit tests
- `MyHomeRamen.IntegrationTests/IdentityModule/Addresses/AddAddressTests.cs`
  - `AddAddress_ShouldReturn201_WithNewAddress` (happy path, `isDefault: false`)
  - `AddAddress_ShouldReturn201_AndSwapDefault_WhenIsDefaultTrue`
  - `AddAddress_ShouldReturn400_WhenUserHas5Addresses`
  - `AddAddress_ShouldReturn401_WhenUnauthenticated`
  - `AddAddress_ShouldReturn400_WhenPayloadInvalid` (theory with invalid data from DataGenerator)

### Notes:
- WebApiFactory needs Identity.Api configuration (UsersDbContext, Keycloak mock)
- Use `HttpClientExtensions.AddAuthorizationHeader()` for auth
- POST to `/api/users/me/addresses`

---

## 9) Architecture tests

Not in scope — no new cross-module boundaries introduced.

---

## 10) System tests

Not in scope — single-service CRUD operation.
