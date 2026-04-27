# refactor RefactorShoppingCartDomain

---

## Title

| Field | Value |
|---|---|
| **Type** | `refactor` |
| **Module** | `ShoppingCart` |
| **Aggregate** | `Basket` · `BasketItem` · `User` (ShoppingCart module) |
| **Accessibility** | `Anonymous · Customer` |
| **Name** | `RefactorShoppingCartDomain` |

---

---

## Description

> As a **developer**, I want to refactor the ShoppingCart domain model so that the Basket properly tracks items with full product details, lifecycle status, and supports guest users — enabling a more accurate and extensible shopping cart experience.

### Scope

| Area | Include? |
|---|---|
| `backend` — Domain + API + Persistence | yes |
| `frontend` — Blazor Server / WASM | no |

### Backend

**Refactor / Optimize:**

**Current behavior and its shortcomings:**
- `Basket` holds a direct relation to `Product`, making it impossible to track individual line items (quantity, price, comment) separately
- `Basket` has no lifecycle status, so it cannot distinguish between an active cart, a checked-out cart, or an abandoned one
- `User` in the ShoppingCart module has no flag to differentiate guest users from registered users, limiting support for anonymous shopping flows

**Desired behavior and expected improvement:**

- **Add `BasketStatus` enum** in the ShoppingCart module with the following values:
  - `Active` — cart is open and being edited
  - `PendingOrder` — cart is awaiting order confirmation
  - `CheckedOut` — cart has been successfully converted to an order
  - `Abandoned` — cart was left inactive and considered lost
  - `Expired` — cart exceeded its valid lifetime

- **Add `BasketItem` aggregate/entity** in the ShoppingCart module with the following properties:
  - `Id` — strongly-typed identifier (`BasketItemId`)
  - `Product` — reference to the product being added
  - `Quantity` — number of units
  - `Price` — price at the time of adding to basket
  - `Comment` — optional customer note for the item (e.g. special preparation instructions)

- **Update `Basket`** aggregate:
  - Remove direct relation to `Product`
  - Add a collection of `BasketItem` (one-to-many)
  - Add `Status` property of type `BasketStatus`

- **Update `User`** (ShoppingCart module only — Users module `User` is out of scope):
  - Add `IsGuest` boolean flag to distinguish guest (anonymous) users from registered users

- **Update Persistence / DbContext configuration**:
  - Add `BasketItemId` strongly-typed value object with a corresponding EF Core value converter and register it in the DbContext
  - Add `BasketItemConfiguration` — EF Core entity configuration for `BasketItem`
  - Update `BasketConfiguration` to reflect the removed `Product` relation, the new `BasketItem` collection, and the `Status` property mapping

- **Domain events to publish**: none

---
