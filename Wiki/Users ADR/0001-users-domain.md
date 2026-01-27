---
title: "ADR-0001: Define Domain Model for Users Module"
status: "Proposed"
date: "2026-01-27"
authors: "MyHomeRamen Team"
tags: ["domain", "users", "architecture"]
supersedes: ""
superseded_by: ""
---

### Status

**Proposed**

### Context

The MyHomeRamen application follows a Modular Monolith architecture. Currently, user-related entities (`User`, `UserId`) are defined independently in multiple modules (`Orders`, `Menu`, `Payments`, `Reservations`, `Basket`). This indicates a need for a dedicated **Users** module to act as the single source of truth for user domain logic (e.g., profiles, preferences, aggressive roots) and to serve as the reference point for other modules.

The current Identity module (`MyHomeRamen.Identity`) handles authentication and authorization using ASP.NET Core Identity. However, mixing business domain logic (like customer loyalty, addresses, or specialized implementation details) with authentication infrastructure is an architectural anti-pattern in Domain-Driven Design.

**Key constraints and requirements:**
- **Modular Monolith**: Modules should be loosely coupled.
- **Strong Typing**: Identifiers must be strongly typed (`UserId`) to avoid primitive obsession.
- **Separation of Concerns**: Authentication mechanism (Identity) should be separate from User Business Domain.
- **Data Consistency**: Other modules need a consistent way to reference users.

### Decision

We will establish a dedicated **Users Domain Model** within the `MyHomeRamen.Domain.Users` namespace.

**The decision includes:**
1.  **Creation of `UserId`**: A strongly typed value object implementing `IEntityId`.
2.  **Creation of `User` Entity**: A sealed aggregate root implementing `IEntity<UserId>` and inheriting from `AuditableEntity`.
3.  **Synchronization**: The Users module will eventually subscribe to integration events (e.g., `UserRegistered`) from the Identity module to create and maintain the domain user record.

**Structure:**
- `MyHomeRamen.Domain.Users.UserId` (Record Struct)
- `MyHomeRamen.Domain.Users.User` (Class)

### Consequences

#### Positive

- **POS-001**: **Centralization**: Provides a single, authoritative location for user lifecycle and business rules.
- **POS-002**: **Decoupling**: Separates authentication (Identity) from business rules, allowing them to evolve independently.
- **POS-003**: **Type Safety**: Establishes a canonical `UserId` type for the domain, reducing bugs related to ID mismatching.

#### Negative

- **NEG-001**: **Data Duplication**: User IDs and basic info (email) will exist in both Identity tables and Users Domain tables, requiring synchronization.
- **NEG-002**: **Complexity**: Adds overhead of maintaining a separate module and synchronization mechanism (e.g., event-driven eventual consistency).
- **NEG-003**: **Initialization**: Requires strategies to ensure a Domain User exists when an Identity User is created.

### Alternatives Considered

#### Use ASP.NET Identity Entities Directly

- **ALT-001**: **Description**: Use `IdentityUser` classes throughout the application domain.
- **ALT-002**: **Rejection Reason**: Creates high coupling between business domain and specific security framework/infrastructure. Harder to unit test domain logic.

#### Shared Kernel User Entity

- **ALT-003**: **Description**: Define `User` in `MyHomeRamen.Domain.Common` for all modules to share.
- **ALT-004**: **Rejection Reason**: Leads to a "God Object" `User` class that methods from all modules depend on, breaking module boundaries and encapsulation.

### Implementation Notes

- **IMP-001**: `UserId` must include implicit operators for `Guid` to facilitate distinct ID generation and compatibility.
- **IMP-002**: `User` entity should encapsulate properties like Name, Email, and Address, using private setters.
- **IMP-003**: Ensure proper integration with `NetArchTest` rules to prevent cyclic dependencies with other modules.

### References

- **REF-001**: `MyHomeRamen.Api.Common.Domain.IEntity`
- **REF-002**: [Project Architecture Guidelines](.github/copilot-instructions.md)
