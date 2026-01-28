---
title: "ADR-0001: Define Domain Model for Reservations Module"
status: "Proposed"
date: "2026-01-28"
authors: "Funigun"
tags: ["domain", "reservations", "architecture"]
---

### Status

**Accepted**

### Context

In MyHomeRamen application Reservations module handles user bookings for tables and time slots.

**Key constraints and requirements:**
- **Modular Monolith**: Module is fully independent, therefore no external service interface is required.
- **Synchronization**: No synchronization with other modules is required at this moment.
- **Independence**: Module should not rely on other modules or domains which will be forces through architecture tests.

**Related modules**
- **User**: Data must be consistent with `User` module when creating reservations.

### Decision

We create a `Reservations` module by combining Clean and Vertical architecture styles by:
- splitting project into several layers (API, Domain, Persistence, Infrastructure),
- following best practices for DDD (aggregates, entities, value objects, repositories, domain services),
- orginizing business capabilities by module and feature folders

**The decision includes:**
1. **Domain**: Domain model will be defined in `MyHomeRamen.Domain` project under `Reservations` folder
2. **Database**: `IOrdersDbContext` will be defined in `MyHomeRamen.Domain` project with implementation in `MyHomeRamen.Persistance` project.
3. **Synchonization**: Module is independent, so no synchronization mechanism is required at this moment.
4. **Stick to assumptions**: Proper architecture tests will be created to ensure that `Reservations` module does not depend directly on other modules (domain, business logic).

### Consequences

#### Positive
- **POS-001**: **Centralization**: Provides a single, authoritative location for Reservation lifecycle and business rules.
- **POS-002**: **Decoupling**: Separates bookings management from other modules, allowing them to evolve independently.

#### Negative
- **NEG-001**: **Isolation**: Lack of synchronization with other modules may lead to challenges if future requirements demand integration.