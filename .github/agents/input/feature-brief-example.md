# feature CreateToDoItem

---

## Title

| Field | Value |
|---|---|
| **Type** | `feature` |
| **Module** | `Orders` (producer) · `ShoppingCart` (consumer) |
| **Aggregate** | `ToDoItem` |
| **Accessibility** | `Employee · Customer` |
| **Name** | `CreateToDoItem` |

---

---

## Description

> As an **Employee** or **Customer**, I want to create a task item linked to an order so that I can track outstanding work within the Orders module and have other modules notified reliably.

### Scope

| Area | Include? |
|---|---|
| `backend` — Domain + API + Persistence | yes |
| `frontend` — Blazor Server / WASM | yes |

### Backend

**Feature / Change:**

- Introduce a `ToDoItem` aggregate in the **Orders** module with the following data: title, description, due date, status, and author user reference
- Introduce the **Transactional Outbox / Inbox** pattern as the reliable messaging foundation for cross-module events:
  - On `ToDoItem` creation, a `CreatedTaskEvent` is atomically saved to an outbox table within the same DB transaction
  - A background outbox relay polls unprocessed outbox entries and publishes them to the message broker (RabbitMQ)
  - The **ShoppingCart** module consumes the integration event via an inbox table and processes it within its own domain
- **API endpoint**: `POST /api/orders/tasks` — authenticated (Bearer token), `Employee` or `Customer` role required; returns `201 Created` with a `Location` header
- **Request shape**: title, description, due date, optional related order ID
- **Domain validation rules**:
  - Title is required and must not exceed 200 characters
  - Due date, when provided, must be a future date
- **API-level validation rules**:
  - Title: required, max 200 characters
  - Due date: optional, must be a valid future date
  - Related order ID: optional, must reference an existing order if provided
- **Domain events to publish**: `CreatedTaskEvent` → consumed by **ShoppingCart** module as `CreatedTaskIntegrationEvent`

### Frontend

**Feature / Change:**

- New page at `/orders/tasks/create`, accessible to authenticated `Employee` and `Customer` users
- New `CreateToDoItemForm` Blazor component with fields: title, description, due date, optional order reference
- On success: redirect to task list / detail page and show a confirmation toast
- On error: display validation messages inline and surface API error responses

---
