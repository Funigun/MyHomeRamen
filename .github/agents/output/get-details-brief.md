# Feature Brief — GetDetails (User Profile)

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Users` |
| **Accessibility** | `Manager · Employee · Customer` (authenticated users only) |
| **Feature name** | `GetDetails` |
| **Short backend description** | A new `GET /api/users/me` endpoint that returns the authenticated user's base profile information: nickname (username), first name, last name, and email. Requires adding an `Email` property to the `User` domain entity and ensuring it is persisted. The `User.Create` factory already accepts `email` as a parameter — it only needs to be mapped to the new property. |
| **Short frontend description** | A Blazor component/page that displays the current user's profile details (nickname, first name, last name, email), fetched from the new endpoint upon load. |
| **Reference feature** | `GetEmployeesEndpoint` (Identity.Api Admin group) · `RegisterEndpoint` · `RegisterEmployeeEndpoint` |

---

## 2) Scope

| Scope | Include? |
|---|---|
| `backend` — API + Domain + Persistence | yes |
| `frontend` — Blazor Server / WASM | yes |

---

## 3) Feature description (Backend scope)

### Domain change — Users module

- Add `Email` property to `User` entity (`MyHomeRamen.Domain/Users/User.cs`) so the email address is explicitly tracked on the domain model (currently the field is inherited from `IdentityUser<Guid>` via `Email`, but it is not surfaced as a first-class domain property)
- No new domain events or value objects required

### New API endpoint

- **Endpoint**: `GET /api/users/me`
- **Group**: `Account`
- **Authentication**: required (Bearer token via Keycloak)
- **Authorization**: any authenticated role (Manager, Employee, Customer)
- **Response**: `200 OK` — `GetDetailsResponse` record containing `Username`, `FirstName`, `LastName`, `Email`
- **Data source**: `IUsersDbContext` — look up the `User` record by `KeycloakUserId` resolved from `ICurrentUser`
- **Reference**: `GetEmployeesEndpoint`, `GetIngredientByIdEndpoint`

### Notes for planning agent

- `RegisterEmployeeEndpoint` and `RegisterEndpoint` will need to be reviewed to ensure the `Email` field is correctly stored when the new `Email` property is added to `User`. Coordinate with the persistence migration if the column is currently provided only by the `IdentityUser<Guid>` base class mapping.

---

## 4) Feature description (Frontend scope)

### New page / component

- **Route**: `/users/me`
- **Access**: authenticated users only
- **Component**: `UserDetailsCard` — displays username, first name, last name, and email in a read-only layout
- **Load behaviour**: calls `GetDetailsAsync()` on page initialisation; shows a loading indicator while the request is in flight and an error message if it fails

### API client

- Extend or create an `AccountApiClient` in the Blazor project (pattern: `MenuApiClient`)
- Method: `GetDetailsAsync()` returning a `GetDetailsResponse` model

---

## 5) Testing Requirements

### Unit tests

**In scope.** The `Email` property addition to `User` should be covered.

Tests to create:
- `IdentityUserValidationTests` — verify that `User.Create(...)` correctly sets `Email`, `FirstName`, `LastName`, `UserName`, and `Role` properties, and that missing/invalid values raise appropriate errors

Reference: `MyHomeRamen.UnitTests/MenuModule/Users/UserValidationTests.cs`

### Integration tests

**In scope.** The new endpoint should be tested end-to-end.

Tests to create:
- `GetDetailsTests` — authenticated request returns `200 OK` with correct user details; unauthenticated request returns `401 Unauthorized`

Reference: existing Identity.Api integration test patterns (TestContainers + Keycloak)
