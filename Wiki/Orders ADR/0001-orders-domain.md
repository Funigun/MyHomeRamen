---
title: "ADR-0001: Define Domain Model for Orders Module"
status: "Accepted"
date: "2026-01-28"
authors: "Funigun"
tags: ["domain", "orders", "architecture"]
---

### Status

**Accepted**

### Context

In MyHomeRamen application Orders module orders manages the lifecycle of customer orders based on `Basket` module.
It must ensure data consistency with `Menu` module when creating orders and collect all necessary information for order processing.

**Key constraints and requirements:**
- **Modular Monolith**: Module exposes `IExternalOrderService` to be consumed by other modules.
- **Synchronization**: It acts as input for `Payments` module, so provided data must be consistent betweend these modules.
- **Independence**: Module should not rely on other modules or domains which will be forces through architecture tests.

**Related modules**
- **Basket**: Module serves the input data when order processing is started. 
- **Menu**: Data consistency must be ensured when creating orders based on basket data. 
- **Payments**: Needs to pass order data when proceeding to payment within `Payments` module. It will store its own copy of order data to ensure payment integrity.
- **User**: Data must be consistent with `User` module when creating order.

### Decision

We create a `Orders` module by combining Clean and Vertical architecture styles by:
- splitting project into several layers (API, Domain, Persistence, Infrastructure),
- following best practices for DDD (aggregates, entities, value objects, repositories, domain services),
- orginizing business capabilities by module and feature folders

**The decision includes:**
1. **Domain**: Domain model will be defined in `MyHomeRamen.Domain` project under `Orders` folder
2. **Database**: `IOrdersDbContext` will be defined in `MyHomeRamen.Domain` project with implementation in `MyHomeRamen.Persistance` project.
3. **Synchonization**: Data must be consistent with `Menu` module when adding items to basket and before proceeding to checkout. No synchronization mechanism is required at this moment.
4. **Stick to assumptions**: Proper architecture tests will be created to ensure that `Order` module does not depend directly on other modules (domain, business logic).

### Consequences

#### Positive
- **POS-001**: **Centralization**: Provides a single, authoritative location for Orders lifecycle and business rules.
- **POS-002**: **Decoupling**: Separates rders management from other modules, allowing them to evolve independently.
- **POS-003**: **Simple synchronization**: Data must be consistent with `Menu` and `Payments` modules only at specific points (products existance, proceeding to payment), reducing complexity.

#### Negative
- **NEG-001**: **Increased Responsibility**: Other modules must handle their own data integrity and consistency for order information.