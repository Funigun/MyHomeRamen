# Feature Brief — DeleteAddress

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Users` |
| **Accessibility** | `Customer · Employee · Admin` (any authenticated user) |
| **Feature name** | `DeleteAddress` |
| **Short backend description** | New `DELETE /users/me/addresses/{id}` endpoint in the Identity.Api that removes an address belonging to the authenticated user. No Keycloak involvement — purely a database delete via `IUsersDbContext`. Existence and ownership of the address are validated inside `ValidationPolicy` before the handler executes. |
| **Short frontend description** | Not in scope for this iteration. |
| **Reference feature** | `AddAddress` (Identity.Api — `Features/Account/Addresses/AddAddress`) · `UpdateAddress` (Identity.Api — `Features/Account/Addresses/UpdateAddress`) |

---

## 2) Scope

| Scope | Include? |
|---|---|
| `backend` — API + Domain + Persistence | yes |
| `frontend` — Blazor Server / WASM | no |

---

## 3) Feature description (Backend scope)

### New API endpoint

- **Endpoint**: `DELETE /users/me/addresses/{id}`
- **Group**: `Account`
- **Authentication**: required (Bearer token via Keycloak)
- **Authorization**: any authenticated role (`Customer`, `Employee`, `Admin`)
- **Request**: no body — `{id}` route parameter identifies the address; authenticated user identity is extracted from JWT claims
- **Response**: `204 No Content` on success; `400 Bad Request` when the address does not exist or does not belong to the authenticated user (validated in `ValidationPolicy`); `401 Unauthorized` for unauthenticated requests
- **Reference**: `UpdateAddressEndpoint`

### Domain changes

- Add a `RemoveAddress` method to `User` entity that removes the given `Address` from the internal `_addresses` collection
- If the removed address was the default address (`IsDefault = true`), the method does **not** automatically promote another address — the user is left with no default
- Reference: `User.AddAddress`, `User.UpdateAddress`

### Persistence changes

- No migration required — cascading delete is already configured for `Addresses`
- `ValidationPolicy` queries `IUsersDbContext` to verify the address exists and its `UserId` matches the authenticated user's ID before the handler is invoked
- Handler loads `User` with addresses, calls `User.RemoveAddress`, and saves

### Contracts

- New request model: `DeleteAddressRequest` record containing only the route-bound `AddressId` (Guid) — no body fields
- Validation via FluentValidation in `ValidationPolicy`: checks the address exists in the database and belongs to the requesting user
- Reference: `AddAddressRequest`, `AddAddressRequestValidator`

---

## 4) Feature description (Frontend scope)

Not in scope for this iteration.

---

## 5) Testing Requirements

### Unit tests

**In scope.** Validate domain `User.RemoveAddress` logic and handler.

Tests to create:
- `UserRemoveAddressTests_ShouldRemoveAddress_WhenExists`
- `UserRemoveAddressTests_ShouldNotChangeOtherAddresses_WhenNonDefaultIsRemoved`
- `UserRemoveAddressTests_ShouldLeaveNoDefault_WhenDefaultAddressIsRemoved`
- `DeleteAddressHandler_ShouldDeleteAddress_WhenValid`

Reference: `MyHomeRamen.UnitTests` — existing domain and handler test patterns

---

### Integration tests

**In scope.** Verify the HTTP endpoint, auth enforcement, `ValidationPolicy` checks, and persistence.

Tests to create:
- `DeleteAddress_ShouldReturn204_WhenAddressExists` (happy path)
- `DeleteAddress_ShouldReturn204_WhenDefaultAddressIsDeleted` (no default promoted)
- `DeleteAddress_ShouldReturn400_WhenAddressNotFound`
- `DeleteAddress_ShouldReturn400_WhenAddressBelongsToAnotherUser`
- `DeleteAddress_ShouldReturn401_WhenUnauthenticated`

Reference: `MyHomeRamen.IntegrationTests` — existing endpoint test patterns

---

### Architecture tests

**Not in scope.** No new cross-module boundaries introduced.

---

### System tests

**Not in scope.** Single-service CRUD operation — no distributed flow.

---

## 6) Additional Notes

- Existence and ownership checks are intentionally placed in `ValidationPolicy` (not the handler) to keep the handler focused on the domain operation and to align with the validation pattern established in `AddAddress` and `UpdateAddress`.
- Deleting the default address does **not** auto-promote another address to default — the caller is responsible for setting a new default via `UpdateAddress` if needed.

---
