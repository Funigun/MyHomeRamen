---
title: "ADR-0001: Define Domain Model for Menu Module"
status: "Accepted"
date: "2026-01-28"
authors: "Funigun"
tags: ["domain", "products", "menu", "architecture"]
---

### Status

**Accepted**

### Context

In MyHomeRamen application Menu module acts the single source of truth for product domain that serves other modules (`Orders`, `Basket`).

This module handles product catalog management, including categories, ingredients and pricing.

**Key constraints and requirements:**
- **Modular Monolith**: Module exposes `IExternalMenuService` to be consumed by other modules.
- **Synchronization**: At this moment we do not expect product data to change during working hours, so any synchronization mechanism is not required (e.g. in case of price change).
- **Independence**: Module should not rely on other modules or domains which will be forces through architecture tests.


**Related modules**
- **Basket**: Needs to retrieve product details for displaying items in the basket. 
              It will store its own copy of product data to ensure basket integrity and handle discounts.

- **Orders**: Needs to retrieve product details for order processing. It will store its own copy of product data to ensure order integrity.
              This will also allow to check hitorical orders even if product data changes later.

- **User**: Data must be consistent with `User` module when managing products.

### Decision

We create a `Menu` module by combining Clean and Vertical architecture styles by:
- splitting project into several layers (API, Domain, Persistence, Infrastructure),
- following best practices for DDD (aggregates, entities, value objects, repositories, domain services),
- orginizing business capabilities by module and feature folders

**The decision includes:**
1. **Domain**: Domain model will be defined in `MyHomeRamen.Domain` project under `Menu` folder
2. **Database**: `IMenuDbContext` will be defined in `MyHomeRamen.Domain` project with implementation in `MyHomeRamen.Persistance` project.
3. **Synchonization**: Other modules will maintain their own copy of product data to ensure integrity. No synchronization mechanism is required at this moment.
4. **Stick to assumptions**: Proper architecture tests will be created to ensure that `Menu` module does not depend directly on other modules (domain, business logic).
5. **Skip discounts**: Current discount scenarios are based on items in whole shopping cart, so they will be handled in `Basket` module.

### Consequences

#### Positive
- **POS-001**: **Centralization**: Provides a single, authoritative location for product lifecycle and business rules.
- **POS-002**: **Decoupling**: Separates product management from other modules, allowing them to evolve independently.
- **POS-003**: **Simplicity**: Avoids complexity of synchronization mechanisms by allowing other modules to maintain their own copies of product data.

#### Negative
- **NEG-001**: **Increased Responsibility**: Other modules must handle their own data integrity and consistency for product information.
- **NEG-002**: **Limited design**: Product data will not be prepared for discount calculations, so any future changes in discount scenarios might require changes in `Menu` module.

#### Rejected negative
Product data serve different purposes in different modules, so data duplication is acceptable (e.g. when price changes, historical orders must retain original price),
therefore the following negative consequences were rejected:

- **NEG-001**: **Data Duplication**
- **NEG-002**: **Stale Data Risk**