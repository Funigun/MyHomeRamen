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

In MyHomeRamen application Menu module acts the single source of truth for product domain that serves other modules (`Orders`, `Shopping Cart`).

This module handles product catalog management, including categories, ingredients and pricing.

**Key constraints and requirements:**
- **Modular Monolith**: Module exposes `IExternalMenuService` to be consumed by other modules.
- **Synchronization**: Product and offer prices or availability can change while a basket exists. Shopping Cart must revalidate the offer and current price through the Menu contract at checkout and notify the customer of price changes. Continuous background synchronization is not required.
- **Independence**: Module should not rely on other modules or domains which will be forces through architecture tests.


**Related modules**
- **Shopping Cart**: Needs to retrieve product details for displaying items in the shopping cart. 
              It may store a display snapshot, but must revalidate offer availability and current prices through the Menu contract before checkout and notify the customer when a saved price changed.

- **Orders**: Needs to retrieve product details for order processing. It will store its own copy of product data to ensure order integrity.
              Checkout must provide restaurant, currency, quantity, and final charged line prices; Orders retain these charged facts so historical orders do not depend on later Menu changes.

- **User**: Data must be consistent with `User` module when managing products.

### Decision

We create a `Menu` module by combining Clean and Vertical architecture styles by:
- splitting project into several layers (API, Domain, Persistence, Infrastructure),
- following best practices for DDD (aggregates, entities, value objects, repositories, domain services),
- orginizing business capabilities by module and feature folders

**The decision includes:**
1. **Domain**: Domain model will be defined in `MyHomeRamen.Domain` project under `Menu` folder
2. **Database**: `IMenuDbContext` will be defined in `MyHomeRamen.Domain` project with implementation in `MyHomeRamen.Persistance` project.
3. **Synchronization**: Other modules may keep their own product display snapshots. Shopping Cart revalidates offers and prices at checkout through `IExternalMenuService`; price changes are disclosed before applying the current price without separate confirmation. No continuous background synchronization is required.
4. **Stick to assumptions**: Proper architecture tests will be created to ensure that `Menu` module does not depend directly on other modules (domain, business logic).
5. **Cart-level discounts**: Discounts based on the contents of the whole shopping cart are handled in `Shopping Cart`. Restaurant offer price adjustments are separate Menu pricing and may be negative within the approved minimum-price floor.

### Consequences

#### Positive
- **POS-001**: **Centralization**: Provides a single, authoritative location for product lifecycle and business rules.
- **POS-002**: **Decoupling**: Separates product management from other modules, allowing them to evolve independently.
- **POS-003**: **Simplicity**: Avoids continuous background synchronization while using contract-based checkout revalidation to catch stale basket data.

#### Negative
- **NEG-001**: **Increased Responsibility**: Other modules must handle their own data integrity and consistency for product information.
- **NEG-002**: **Stale basket snapshots**: Basket display data can become stale before checkout, so Shopping Cart must revalidate and disclose price changes.

#### Rejected negative
Product data serve different purposes in different modules, so data duplication is acceptable (e.g. when price changes, historical orders must retain original price),
therefore the following negative consequences were rejected:

- **NEG-001**: **Data Duplication**