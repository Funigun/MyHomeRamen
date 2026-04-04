# Feature Brief — CreateToDoItem (Transactional Outbox / Inbox Pattern)

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Orders` (producer) · `ShoppingCart` (consumer) |
| **Accessibility** | `Employee · Customer` (authenticated users only) |
| **Feature name** | `CreateToDoItem` |
| **Short backend description** | Introduce the Transactional Outbox / Inbox pattern: a new `POST /api/orders/tasks` endpoint creates a `ToDoItem` entity in the **Orders** module and atomically persists a `CreatedTaskEvent` to an outbox table. A background worker (outbox relay) reads unprocessed outbox entries and publishes them to the message broker (RabbitMQ). A second background worker in the **ShoppingCart** module consumes the event from the broker, saves it to a module-local inbox table, and processes it within its own domain. |
| **Short frontend description** | A new Blazor page with a form that allows an authenticated user (Employee or Customer) to create a task item, with real-time status feedback. |
| **Reference feature** | `CreateOrder` (Orders module) · existing `UserRegisteredIntegrationEvent` outbox-less flow (Worker.MessagesHandler) |

---

## 2) Scope

| Scope | Include? |
|---|---|
| `backend` — API + Domain + Persistence | yes |
| `frontend` — Blazor Server / WASM | yes |

---

## 3) Feature description (Backend scope)

### New flow overview

```
[Client]
   │
   ▼ POST /api/orders/tasks
[CreateToDoItem Endpoint]
   │
   ▼
[CreateToDoItemHandler]
   ├── creates ToDoItem domain entity
   ├── raises CreatedTaskEvent (IDomainEvent)
   └── saves ToDoItem + outbox entry atomically
          │
          ▼
[Orders DB — OutboxMessages table]
          │
          ▼ (polling / Quartz job)
[Outbox Relay Worker — Worker.MessagesHandler or new hosted service]
          │
          └── publishes CreatedTaskIntegrationEvent → RabbitMQ (task-events-queue)
                         │
                         ▼ (consume)
          [ShoppingCart Inbox Worker — Worker.MessagesHandler]
                         │
                         ├── saves message to InboxMessages table (ShoppingCart DB)
                         └── processes event (e.g. updates basket state)
```

### New API endpoint

- **Endpoint**: `POST /api/orders/tasks`
- **Authentication**: required (Bearer token via Keycloak)
- **Authorization**: `Employee` or `Customer` role
- **Request**: task title, description, due date, related order ID (optional)
- **Response**: `201 Created` with `Location` header pointing to the new resource
- **Reference**: `CreateOrderEndpoint`, `CreateCategoryEndpoint`

### Domain changes — Orders module

- New domain entity: `ToDoItem` with `ToDoItemId`, title, description, status, author user reference
- New domain event: `CreatedTaskEvent` implementing `IDomainEvent`
- New outbox entity: `OutboxMessage` (generic envelope for any serialised domain event)
- Reference: `OrderCreatedEvent`, `OrderAcceptedEvent`

### Persistence changes — Orders module

- New `OutboxMessage` EF Core entity configuration and migration in `MyHomeRamen.Persistance/Orders/`
- `IOrdersDbContext` extended with `DbSet<OutboxMessage>`
- Reference: `OrderConfiguration`, `OrdersDbContext`

### Persistence changes — ShoppingCart module

- New `InboxMessage` EF Core entity configuration and migration in `MyHomeRamen.Persistance/ShoppingCart/`
- `IShoppingCartDbContext` extended with `DbSet<InboxMessage>`

### Contracts

- New integration event record in `MyHomeRamen.Common.Contracts`: `CreatedTaskIntegrationEvent`
- Reference: `UserRegisteredIntegrationEvent`

### Worker changes

- New outbox relay: a `BackgroundService` (or Quartz job in `MyHomeRamen.Worker.MessagesHandler`) that periodically polls `OutboxMessages`, publishes unprocessed entries to the message broker, and marks them as processed
- New inbox consumer: a new `IIntegrationEventHandler<CreatedTaskIntegrationEvent>` in the ShoppingCart section of `MyHomeRamen.Worker.MessagesHandler` that saves the event to the inbox table and processes it
- Reference: `ReservationsUserRegisteredHandler`, `Worker.cs` (existing consumer loop)

---

## 4) Feature description (Frontend scope)

### New page

- **Route**: `/orders/tasks/create`
- **Access**: authenticated users only (Employee or Customer)
- **Component**: `CreateToDoItemForm` — Blazor interactive form with fields for title, description, due date, and an optional order reference
- **Success behaviour**: redirects to a task list or detail page and shows a confirmation toast/alert
- **Error behaviour**: displays validation errors inline and API error messages

### API client

- Extend or create a `TasksApiClient` in the Blazor project (pattern: `MenuApiClient`)
- Method: `CreateToDoItemAsync(CreateToDoItemRequest request)`

---

## 5) Testing Requirements

### Unit tests

**In scope.** The new domain entity and domain event carry validation logic that must be tested in isolation.

Tests to create:
- `ToDoItemValidationTests` — valid/invalid task creation scenarios (empty title, null description, past due date, etc.)
- `CreatedTaskEventTests` — verifies the event carries the correct aggregate reference

Reference: `MyHomeRamen.UnitTests/OrdersModule/Orders/OrderValidationTests.cs`, `MyHomeRamen.UnitTests/MenuModule/Categories/CategoryValidationTests.cs`

---

### Integration tests

**In scope.** The HTTP endpoint, handler, outbox write, and authorisation rules must be tested with a real DB via TestContainers.

Tests to create:
- `CreateToDoItemTests` (happy path): `POST /api/orders/tasks` returns `201 Created` and the outbox table contains one unprocessed entry
- `CreateToDoItem_ShouldReturnUnauthorized_ForUnauthenticatedUser`
- `CreateToDoItem_ShouldReturnForbidden_ForInvalidRole` (e.g. anonymous or wrong role)
- `CreateToDoItem_ShouldReturnBadRequest_ForInvalidPayload` (missing required fields)

Reference: `MyHomeRamen.IntegrationTests/MenuModule/CreateCategoryTests.cs`, `MyHomeRamen.IntegrationTests/OrdersModule/CreateOrderTests.cs`

---

### Architecture tests

**In scope.** The outbox/inbox infrastructure introduces new cross-module data flows. Boundaries must be enforced.

Tests to create:
- `OrdersApi_ShouldNot_DependOn_ShoppingCartApi` (or other consumer modules) — existing pattern should be verified for the new feature
- `OutboxMessage_ShouldBelong_ToOrdersNamespace` — verify the outbox entity lives only in the Orders persistence namespace
- `InboxMessage_ShouldBelong_ToShoppingCartNamespace` — verify the inbox entity lives only in the ShoppingCart persistence namespace
- Verify `CreatedTaskIntegrationEvent` is defined only in `MyHomeRamen.Common.Contracts`

Reference: `MyHomeRamen.ArchitectureTests/ModuleTests/Orders/ApiBoundariesTests.cs`, `MyHomeRamen.ArchitectureTests/ModuleTests/Orders/PersistanceBoundariesTests.cs`

---

### System tests

**In scope.** The complete distributed flow (API → outbox → worker → RabbitMQ → inbox → handler) requires full orchestration to be validated end-to-end.

Tests to create:
- `CreateToDoItem_ShouldPublishCreatedTaskEvent_ToMessageBroker` — call the API, wait for the outbox relay to fire, assert the message appears in RabbitMQ
- `CreatedTaskIntegrationEvent_ShouldBeProcessed_ByShoppingCartInboxHandler` — assert the inbox table in the ShoppingCart DB contains the processed entry after a configurable timeout

Reference: `MyHomeRamen.SystemTests` project (Aspire.Hosting.Testing orchestration)

---

## 6) Additional Notes

- **Outbox/Inbox pattern justification**: The current `MessagesService` publishes directly to RabbitMQ without transactional guarantees. This feature introduces a reliable messaging foundation (Transactional Outbox + Inbox) to prevent message loss on API or broker failure, and serves as the reference implementation for future cross-module events.
- **Idempotency**: The inbox handler should use the `InboxMessage.Id` (mapped from the outbox entry `Id`) to deduplicate redelivered messages.
- **Polling vs. CDC**: The initial implementation uses a simple polling-based outbox relay. Change Data Capture (CDC) or a dedicated Outbox library (e.g. CAP, Wolverine) may be evaluated as a follow-up.
- **Queue naming**: A new dedicated queue (e.g. `task-events-queue`) should be used, separate from the existing `user-events-queue`, to avoid consumer interference.
- **Authorization**: The endpoint must be protected by a Keycloak policy; ensure `PermissionConstants` and role mappings are updated for the Orders module.

---
