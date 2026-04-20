# Feature Brief — DeleteAccount

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Users` |
| **Accessibility** | `Customer · Employee · Admin` (any authenticated user) |
| **Feature name** | `DeleteAccount` |
| **Short backend description** | New `DELETE /users/me/delete` endpoint in the Identity.Api that deletes the authenticated user's Keycloak account via the Admin API, removes the `User` and associated `Address` rows from the identity database, and publishes a `UserDeletedIntegrationEvent` so other modules (Orders, Payments, Reservations, ShoppingCart, Menu) can clean up their local user projections. |
| **Short frontend description** | Not in scope for this iteration. |
| **Reference feature** | `Register` (Identity.Api — `Features/Account/Register`) · `RegisterEmployee` (Identity.Api — `Features/Admin/RegisterEmployee`) |

---

## 2) Scope

| Scope | Include? |
|---|---|
| `backend` — API + Domain + Persistence | yes |
| `frontend` — Blazor Server / WASM | no |

---

## 3) Feature description (Backend scope)

### New API endpoint

- **Endpoint**: `DELETE /users/me/delete`
- **Group**: `Account`
- **Authentication**: required (Bearer token via Keycloak)
- **Authorization**: any authenticated role (`Customer`, `Employee`, `Admin`)
- **Request**: no body — user identity is extracted from the JWT claims
- **Response**: `204 No Content` on success
- **Reference**: `RegisterEndpoint`, `RegisterEmployeeEndpoint`

### Infrastructure changes — Keycloak Admin Service

- New method on `IKeycloakAdminService`: `Task DeleteUserAsync(string keycloakUserId, CancellationToken cancellationToken)`
- Implementation calls Keycloak Admin API: `DELETE /admin/realms/{realm}/users/{keycloakUserId}`
- Reference: `KeycloakAdminService.CreateUserAsync`

### Persistence changes

- The handler loads the `User` (including `Addresses` navigation) from `IUsersDbContext`, removes the entity (cascade-deletes addresses), and calls `SaveChangesAsync`
- No schema migration required — EF cascade delete already configured

### Contracts

- New integration event: `UserDeletedIntegrationEvent(Guid UserId)` in `MyHomeRamen.Common.Contracts`
- Reference: `UserRegisteredIntegrationEvent`

### Worker changes

- New handler in `MyHomeRamen.Worker.MessagesHandler` for each consuming module (Orders, Payments, Reservations, ShoppingCart, Menu) to remove or soft-delete the local user projection on receipt of `UserDeletedIntegrationEvent`
- Reference: existing `UserRegisteredIntegrationEvent` handlers

---

## 4) Feature description (Frontend scope)

Not in scope for this iteration.

---

## 5) Testing Requirements

### Unit tests

**In scope.** Validate the handler logic (Keycloak service called, user removed, event published).

Tests to create:
- `DeleteAccountHandler_ShouldCallKeycloakDeleteUser`
- `DeleteAccountHandler_ShouldRemoveUserFromDatabase`
- `DeleteAccountHandler_ShouldPublishUserDeletedIntegrationEvent`

Reference: `MyHomeRamen.UnitTests` — existing handler tests pattern

---

### Integration tests

**In scope.** Verify the HTTP endpoint, auth enforcement, and database state.

Tests to create:
- `DeleteAccount_ShouldReturn204_WhenAuthenticated` (happy path)
- `DeleteAccount_ShouldReturn401_WhenUnauthenticated`

Reference: `MyHomeRamen.IntegrationTests` — existing endpoint test patterns

---

### Architecture tests

**Not in scope.** No new cross-module boundaries introduced beyond the existing integration event pattern.

---

### System tests

**In scope.** The full flow (API → Keycloak deletion → DB removal → integration event → consumer handlers) spans multiple services.

Tests to create:
- `DeleteAccount_ShouldRemoveKeycloakUser_AndPublishEvent`
- `UserDeletedIntegrationEvent_ShouldBeProcessed_ByConsumerModules`

Reference: `MyHomeRamen.SystemTests` (Aspire.Hosting.Testing)

---

## 6) Additional Notes

- The handler must delete the Keycloak user **first**; if the Keycloak call fails the local DB should remain untouched (no partial deletion).
- Consider whether a soft-delete strategy is more appropriate for consumer modules to preserve referential integrity in historical orders/payments.

---
