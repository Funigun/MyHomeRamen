---
title: "ADR-0003: Isolated Module Database Contexts"
status: "Accepted"
date: "2025-01-30"
authors: "Funigun"
tags: ["architecture", "decision", "database", "persistence"]
---

### Status

**Accepted**

### Context

In our Modular Monolith, different modules (Orders, Menu, Identity, etc.) represent distinct business subdomains.

Sharing a single `DbContext` across all modules would violate the principle of loose coupling. 
It would encourage cross-module queries (breaking module boundaries), lead to a massive monolithic Context class that is hard to maintain, and make future extraction of modules into microservices significantly harder (as the database schema would be entangled).

However, we still need a consistent way to manage database transactions and persistence operations across these disparate contexts to ensure data integrity and standardize the Unit of Work pattern.

### Decision

We will enforce **Database Isolation per Module** with a shared contract.

Key aspects of this decision:
1.  **Separate DbContexts**: Each module will possess its own Entity Framework Core `DbContext` (e.g., `OrdersDbContext`, `MenuDbContext`).
2.  **Schema Separation**: Each DbContext will map to a specific database schema (e.g., `orders.*`, `menu.*`) within the same physical database (initially), or different databases if needed. This logical separation prevents table joins across modules.
3.  **Shared Contract**: All module DbContexts must implement the `IBaseDbContext` interface defined in `MyHomeRamen.Api.Common`.
    - This interface enforces standard methods: `SaveChangesAsync`, `BeginTransaction`, `CommitTransaction`, and `RollbackTransaction`.
    - This allows infrastructure components (like transaction behaviors or unit of work pipelines) to interact with any module's database context polymorphically.
4.  **No Cross-Context Navigation**: Entity navigation properties must not span across module boundaries. Relationships between modules are handled via IDs (Logical Foreign Keys), not object references.

### Consequences

#### Positive

- **POS-001**: **Decoupling**: Changes to one module's schema generally do not affect others.
- **POS-002**: **Migration Path**: Since data is already logically separated by schema/context, moving a module's data to a separate physical database for a microservice transition is straightforward.
- **POS-003**: **Maintainability**: Smaller, focused DbContexts are easier to understand and optimize than one giant context.
- **POS-004**: **Consistency**: Implementing `IBaseDbContext` ensures all modules support the same transactional primitives required by our application pipeline.

#### Negative

- **NEG-001**: **Boilerplate**: Requires creating and configuring a `DbContext` subclass for every module.
- **NEG-002**: **No Cross-Module Joins**: Queries that need data from multiple modules cannot rely on EF Core Include/Joins. They must be handled via data composition at the API level or via event duplication/read models (CQRS).
- **NEG-003**: **Transaction Management**: Atomicity across modules is not automatic (unless using distributed transaction patterns or accepting eventual consistency), though initially sharing one physical DB allows for transaction scopes if carefully managed.

### Alternatives Considered

#### Single Shared DbContext

- **ALT-001**: **Description**: One `AppDbContext` containing all `DbSet`s for the entire application.
- **ALT-001**: **Rejection Reason**: Violates modularity. Encourages spaghetti code where everything connects to everything. Makes application startup slower as the model grows.

#### Generic Repository Pattern Only

- **ALT-002**: **Description**: Hide EF Core completely behind generic repositories without exposing specific Contexts.
- **ALT-002**: **Rejection Reason**: Often leads to "lowest common denominator" data access. We prefer the full power of EF Core (DbSet) within the module boundary, controlled by the specific Context.
