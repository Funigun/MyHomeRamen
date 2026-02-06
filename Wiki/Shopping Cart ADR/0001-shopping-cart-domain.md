---
title: "ADR-0001: Define Domain Model for Shopping Cart Module"
status: "Proposed"
date: "2026-01-28"
authors: "Funigun"
tags: ["domain", "shopping cart", "basket", "architecture"]
---

### Status

**Accepted**

### Context

In MyHomeRamen application Shopping Cart module handles collecting products that user wants to order based on `Menu` module.
It will also handle discounts and promotions before proceeding to checkout.

Currently in MyHomeRamen application there are following discounts scenarios to be handled in shopping cart module:
- Percentage-based discounts (e.g., 10% off on total order)
- Fixed amount discounts (e.g., $5 off on orders over $50)
- Buy One Get One Free (BOGO) offers
- Free shipping for orders over a certain amount

This means it will also act as source of truth for current shopping cart state before an order is created in `Orders` module.

**Key constraints and requirements:**
- **Modular Monolith**: Module exposes `IExternalShoppingCartService` to be consumed by other modules.
- **Synchronization**: It acts as input for `Orders` module when proceeding to checkout. It must make sure that provided data is consistent with `Menu` module at the moment of order creation.
- **Independence**: Module should not rely on other modules or domains which will be forces through architecture tests.

**Related modules**
- **Menu**: Product data must be consistent with `Menu` module when adding items to shopping cart. 
            It will store its own copy of product data to ensure shopping cart integrity and handle discounts.

- **Orders**: Needs to pass shopping cart data when proceeding to checkout within `Orders` module. It will store its own copy of product data to ensure order integrity.

- **User**: Data must be consistent with `User` module when managing shopping cart.

### Decision

We create a `ShoppingCart` module by combining Clean and Vertical architecture styles by:
- splitting project into several layers (API, Domain, Persistence, Infrastructure),
- following best practices for DDD (aggregates, entities, value objects, repositories, domain services),
- orginizing business capabilities by module and feature folders

**The decision includes:**
1. **Domain**: Domain model will be defined in `MyHomeRamen.Domain` project under `ShoppingCart` folder
2. **Database**: `IMenuDbContext` will be defined in `MyHomeRamen.Domain` project with implementation in `MyHomeRamen.Persistance` project.
3. **Synchonization**: Data must be consistent with `Menu` module when adding items to shopping cart and before proceeding to checkout. No synchronization mechanism is required at this moment.
4. **Stick to assumptions**: Proper architecture tests will be created to ensure that `ShoppingCart` module does not depend directly on other modules (domain, business logic).


### Consequences

#### Positive
- **POS-001**: **Centralization**: Provides a single, authoritative location for shopping cart lifecycle and business rules.
- **POS-002**: **Decoupling**: Separates shopping cart management from other modules, allowing them to evolve independently.
- **POS-003**: **Simple synchronization**: Data must be consistent with `Menu` module only at specific points (adding items, proceeding to checkout), reducing complexity.

#### Negative
- **NEG-001**: **Increased Responsibility**: Other modules must handle their own data integrity and consistency for product information.

