---
title: "ADR-0001: Define Domain Model for Payments Module"
status: "Proposed"
date: "2026-01-28"
authors: "Funigun"
tags: ["domain", "payments", "architecture"]
---

### Status

**Accepted**

### Context

In MyHomeRamen application Payments module handles payment providers integration and payment processing based on `Orders` module.

**Key constraints and requirements:**
- **Modular Monolith**: Module exposes `IExternalPaymentService` to be consumed by other modules.
- **Synchronization**: It must notify `Orders` module about payment status changes, so provided data must be consistent betweend these modules.
- **Independence**: Module should not rely on other modules or domains which will be forces through architecture tests.

**Related modules**
- **Orders**: Module serves the input data when order processing is started.

### Decision

We create a `Payments` module by combining Clean and Vertical architecture styles by:
- splitting project into several layers (API, Domain, Persistence, Infrastructure),
- following best practices for DDD (aggregates, entities, value objects, repositories, domain services),
- orginizing business capabilities by module and feature folders

**The decision includes:**
1. **Domain**: Domain model will be defined in `MyHomeRamen.Domain` project under `Payments` folder
2. **Database**: `IOrdersDbContext` will be defined in `MyHomeRamen.Domain` project with implementation in `MyHomeRamen.Persistance` project.
3. **Synchonization**: Payment status must be provided to `Orders` module when payment status changes. No synchronization mechanism is required at this moment.
4. **Stick to assumptions**: Proper architecture tests will be created to ensure that `Payments` module does not depend directly on other modules (domain, business logic).

### Consequences

#### Positive
- **POS-001**: **Centralization**: Provides a single, authoritative location for Payments lifecycle and business rules.
- **POS-002**: **Decoupling**: Separates payments management from other modules, allowing them to evolve independently.
- **POS-003**: **Simple synchronization**: Data must be consistent with `Orders` module only at specific points (successed, cancelled), reducing complexity.

#### Negative
- **NEG-001**: **Increased Responsibility**: Other modules must handle their own data integrity and consistency for order information.