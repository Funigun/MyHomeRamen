---
title: "ADR-0001: Define Domain Model for Users Module"
status: "Accepted"
date: "2026-01-28"
authors: "Funigun"
tags: ["domain", "users", "architecture"]
---

### Status

**Accepted**

### Context

In MyHomeRamen application Users module acts the single source of truth for user domain that serves other modules (`Orders`, `Menu`, `Payments`, `Reservations`, `Basket`). 
Different modules will require different user-related data and behaviors, so in order to mainain separation of concerns and modularity, we need to define a dedicated Users Domain Model.

This module will handle authentication and authorization and provide features related to user management, such as user profiles, roles, and permissions.
Every user-related data will go throught this module. Other modules will subscribe to events published through a message broker (RabbitMQ) to keep their local user references in sync.

**Key constraints and requirements:**
- **Modular Monolith**: Modules should be loosely coupled.
- **Strong Typing**: Identifiers must be strongly typed (`UserId`) to avoid primitive obsession.
- **Separation of Concerns**: Authentication mechanism (Identity) should be separate from User Business Domain.
- **Data Consistency**: Other modules need a consistent way to reference users.

### Decision

We will create separate API project dedicated for Users management (`MyHomeRamen.Identity.Api`).

**The decision includes:**
1.  **Creation of `UserId`**: A strongly typed value object implementing `IEntityId`.
2.  **Creation of `User` Entity**: A sealed aggregate root implementing `IEntity<UserId>` and inheriting from `AuditableEntity`.
3.  **Synchronization**: The Users module will eventually subscribe to integration events (e.g., `UserRegistered`) from the Identity module to create and maintain the domain user record.
                         Synchronization will be handled asynchronously using RabbitMQ to ensure eventual consistency.
**Structure:**
- `MyHomeRamen.Domain.Users.UserId` (Record Struct)
- `MyHomeRamen.Domain.Users.User` (Class)

### Consequences

#### Positive

- **POS-001**: **Centralization**: Provides a single, authoritative location for user lifecycle and business rules.
- **POS-002**: **Decoupling**: Separates authentication (Identity) from business rules, allowing them to evolve independently.
- **POS-003**: **Type Safety**: Establishes a canonical `UserId` type for the domain, reducing bugs related to ID mismatching.

#### Negative

- **NEG-001**: **Complexity**: Adds overhead of maintaining a separate module and synchronization mechanism (e.g., event-driven eventual consistency).
- **NEG-002**: **Eventual consistency**: Separeted modules might have inconsistent data until synchronization process is finished, however this is acceptable as used data does not change frequently.
