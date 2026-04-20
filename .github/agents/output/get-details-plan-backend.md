# Feature Backend Plan — GetDetails

- **Date**: 2025-07-17
- **Feature**: GetDetails — `GET /users/me`

---

## 1) Domain changes

### Files to modify:
- `MyHomeRamen.Domain/Users/User.cs` — The `Email` property is already inherited from `IdentityUser<Guid>`. Verify it is accessible. If the brief requires a first-class domain property, the `Email` is already set in `User.Create(...)` via the base class property. No additional domain property needed since `IdentityUser<Guid>.Email` is public.

### Assessment:
- `User.Create(...)` already sets `Email`, `UserName`, `FirstName`, `LastName` — all fields needed for the response are present
- No new domain methods or properties required

---

## 2) Persistence changes

No migration required — `Email` column is already part of the `AspNetUsers` table via `IdentityUser<Guid>`.

---

## 3) Contracts

No new contract validators — this is a read-only endpoint with no request body.

---

## 4) API feature — GetDetails

### Folder structure:
```
MyHomeRamen.Identity.Api/
└── Features/
    └── Account/
        └── GetDetails/
            ├── Models/
            │   ├── GetDetailsResponse.cs
            │   └── Mappings.cs
            ├── GetDetailsEndpoint.cs
            └── GetDetailsHandler.cs
```

### Files to create:

#### `GetDetailsResponse.cs`
- `public sealed record GetDetailsResponse(string Username, string FirstName, string LastName, string Email, string PhoneNumber);`

#### `Mappings.cs`
- `extension(User)` → `ToGetDetailsResponse()` mapping to `GetDetailsResponse`

#### `GetDetailsEndpoint.cs`
- `IEndpoint` with `GroupName = "Account"`
- `MapStandardGet<GetDetailsResponse>("/me", Handler)`
- `RequireAuthorization()` (any authenticated user)
- `WithName("GetDetailsEndpoint")`

#### `GetDetailsHandler.cs`
- `public sealed class GetDetailsHandler(IUsersDbContext dbContext, ICurrentUser currentUser)`
- Query `Users` with `AsNoTracking()`, filter by `KeycloakUserId == currentUser.Id`
- Map to `GetDetailsResponse` via `Mappings.ToGetDetailsResponse()`
- Return `200 OK`

---

## 5) Endpoint handler flow

1. Extract `KeycloakUserId` from `ICurrentUser.Id`
2. Query user from `IUsersDbContext.Users` with `AsNoTracking()`
3. Map to `GetDetailsResponse`
4. Return `200 OK` with `GetDetailsResponse`

---

## 7) Create unit tests

### Files to create:
- `MyHomeRamen.UnitTests/UsersModule/Users/IdentityUserValidationTests.cs`
  - `Create_Should_SetEmail_When_DataIsValid`
  - `Create_Should_SetFirstName_When_DataIsValid`
  - `Create_Should_SetLastName_When_DataIsValid`
  - `Create_Should_SetUserName_When_DataIsValid`
  - `Create_Should_SetRole_When_DataIsValid`

Note: These verify `User.Create(...)` correctly maps all properties. Reference: `MyHomeRamen.UnitTests/MenuModule/Users/UserValidationTests.cs`

---

## 8) Create integration tests

### Files to create/modify:
- `MyHomeRamen.IntegrationTests/IdentityModule/Account/GetDetailsTests.cs`
  - `GetDetails_ShouldReturn200_WithCorrectUserDetails`
  - `GetDetails_ShouldReturn401_WhenUnauthenticated`

### Notes:
- Requires a seeded user matching the JWT token's `KeycloakUserId` claim
- Verify response contains `Username`, `FirstName`, `LastName`, `Email`, `PhoneNumber`

---

## 9) Architecture tests

Not in scope.

---

## 10) System tests

Not in scope.
