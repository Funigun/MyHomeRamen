# Feature Backend Plan — DeleteAddress

- **Date**: 2025-07-17
- **Feature**: DeleteAddress — `DELETE /users/me/addresses/{id}`

---

## 1) Domain changes

### Files to modify:
- `MyHomeRamen.Domain/Users/User.cs` — Add `RemoveAddress(Guid addressId)` method that:
  1. Finds the address by ID in `_addresses` — throws `DomainException` if not found
  2. Removes it from `_addresses`
  3. Does **not** auto-promote another address to default if the removed one was default

---

## 2) Persistence changes

No migration required — cascading delete already configured for `Addresses`.

---

## 3) Contracts

### Files to create:
- `MyHomeRamen.Identity.Api/Features/Account/Addresses/DeleteAddress/Models/DeleteAddressRequest.cs`
  - `public sealed record DeleteAddressRequest(Guid Id) : IRequest, IRequestId;`
  - No body fields — only route-bound `Id`

---

## 4) API feature — DeleteAddress

### Folder structure:
```
MyHomeRamen.Identity.Api/
└── Features/
    └── Account/
        └── Addresses/
            └── DeleteAddress/
                ├── Models/
                │   └── DeleteAddressRequest.cs
                ├── Policies/
                │   └── DeleteAddressValidationPolicy.cs
                ├── DeleteAddressEndpoint.cs
                └── DeleteAddressHandler.cs
```

### Files to create:

#### `DeleteAddressValidationPolicy.cs`
- `AbstractValidator<DeleteAddressRequest>` implementing `IValidationPolicy<DeleteAddressRequest>`
- Inject `IUsersDbContext` and `ICurrentUser`
- Validate: address exists in DB and `UserId` matches the authenticated user's ID
- Reference: AddAddress/UpdateAddress validation patterns

#### `DeleteAddressEndpoint.cs`
- `IEndpoint` with `GroupName = "Account"`
- `MapStandardDelete<DeleteAddressRequest>("/me/addresses/{id}", Handler)` with `.WithValidationFilter<DeleteAddressRequest>()`
- `RequireAuthorization()` (any authenticated user)
- `WithName("DeleteAddressEndpoint")`

#### `DeleteAddressHandler.cs`
- `public sealed class DeleteAddressHandler(IUsersDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<DeleteAddressRequest>`
- Load `User` by `KeycloakUserId` with `.Include(u => u.Addresses)`
- Call `user.RemoveAddress(request.Id)`
- `SaveChangesAsync()`
- Return `204 No Content`

---

## 5) Endpoint handler flow

1. ValidationPolicy verifies address exists and belongs to user (returns `400 Bad Request` if not)
2. Handler loads `User` with addresses
3. Calls `user.RemoveAddress(addressId)`
4. `SaveChangesAsync()`
5. Returns `204 No Content`

---

## 7) Create unit tests

### Files to create:
- `MyHomeRamen.UnitTests/UsersModule/Users/UserRemoveAddressTests.cs`
  - `RemoveAddress_Should_RemoveAddress_WhenExists`
  - `RemoveAddress_Should_ThrowDomainException_WhenAddressNotFound`
  - `RemoveAddress_Should_NotChangeOtherAddresses_WhenNonDefaultIsRemoved`
  - `RemoveAddress_Should_LeaveNoDefault_WhenDefaultAddressIsRemoved`

---

## 8) Create integration tests

### Files to create/modify:
- `MyHomeRamen.IntegrationTests/IdentityModule/Addresses/DeleteAddressTests.cs`
  - `DeleteAddress_ShouldReturn204_WhenAddressExists`
  - `DeleteAddress_ShouldReturn204_WhenDefaultAddressIsDeleted`
  - `DeleteAddress_ShouldReturn400_WhenAddressNotFound`
  - `DeleteAddress_ShouldReturn400_WhenAddressBelongsToAnotherUser`
  - `DeleteAddress_ShouldReturn401_WhenUnauthenticated`

---

## 9) Architecture tests

Not in scope.

---

## 10) System tests

Not in scope.
