# feature RegisterGuest

---

## Title

| Field | Value |
|---|---|
| **Type** | `feature` |
| **Module** | `Users` |
| **Aggregate** | `User` |
| **Accessibility** | `Anonymous` |
| **Name** | `RegisterGuest` |

---

---

## Description

> As an **Anonymous** visitor, I want to be automatically registered as a guest so that I can interact with the application (e.g. browse menu, add to cart) without creating a full account, while still being identifiable across requests.

### Scope

| Area | Include? |
|---|---|
| `backend` — Domain + API + Persistence | yes |
| `frontend` — Blazor Server / WASM | yes |

### Backend

**Feature / Change:**

- Modify the `User` aggregate in the **Users** module:
  - Make `KeycloakUserId` nullable (`string?`) to support guest users who are not registered in Keycloak
  - Add a nullable `Guid? GuestId` field to uniquely identify a guest session
- Introduce a new `POST /account/guest` endpoint:
  - Publicly accessible — no authentication required
  - Creates a guest `User` record with a new `GuestId` (Version 7 GUID) and no `KeycloakUserId`
  - Returns `201 Created` with the generated `GuestId`
  - Publishes `GuestUserCreatedIntegrationEvent` after successful creation
- **Domain validation rules**:
  - A `User` must have either `KeycloakUserId` or `GuestId` set — not both, not neither
- **API-level validation rules**:
  - No request body required
  - Idempotency: if a valid guest session cookie is already present in the request, return the existing `GuestId` without creating a new record
- **Domain events to publish**: `GuestUserCreatedIntegrationEvent` — produced by **Users** module; consuming modules (e.g. ShoppingCart) may listen to initialize a guest cart

### Frontend

**Feature / Change:**

- On first anonymous page load (Blazor Server), call `POST /account/guest` if no guest session cookie is present
- Store the returned `GuestId` in an **HttpOnly cookie** (e.g. `guest_id`) via the server-side Blazor response — the cookie must be set server-side to honour the HttpOnly constraint
- The cookie should have a reasonable expiry (e.g. 30 days) and be scoped to the application path
- No new page or user-visible UI is required — this is a transparent background registration
- Subsequent requests from the same anonymous visitor must attach the `guest_id` cookie so the backend can resolve the guest identity without issuing a new guest record

---
