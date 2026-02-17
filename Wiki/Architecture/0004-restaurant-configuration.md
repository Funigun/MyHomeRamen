---
title: "ADR-0004: Centralized Restaurant Configuration"
status: "Accepted"
date: "2026-02-18"
authors: "Funigun"
tags: ["architecture", "decision", "configuration", "deployment"]
---

### Status

**Accepted**

### Context

The "My Home Ramen" solution is designed to manage complete operations for a Ramen restaurant. A key requirement is the ability to easily deploy separate, isolated instances of the application for different restaurants.

Configuring each deployment individually by modifying code or scattering settings across various configuration files is error-prone and inefficient. We need a unified strategy to handle restaurant-specific settings—such as identity, naming conventions, and database connections—that facilitates easy replication of the infrastructure for new clients.

Additionally, to support isolation or potential side-by-side deployments, we need control over resource naming (e.g., for messaging queues or cache keys) and database connectivity per module.

### Decision

We will implement a **Centralized Restaurant Configuration** strategy pattern via a dedicated `RestaurantConfigurationProvider`.

Key aspects of this decision:
1.  **Unified Configuration Section**: All restaurant-specific settings will be grouped under a single configuration section `RestaurantConfiguration` in `appsettings.json` (or Environment Variables).
2.  **Strongly Typed Provider**: Access to these settings will be mediated through `RestaurantConfigurationProvider`, which abstracts the raw `IConfiguration` usage.
3.  **Configurable Scope**:
    - **Identity**: `RestaurantName` and `RestaurantId` (GUID) to uniquely identify the deployment.
    - **Infrastructure Isolation**: An `InfrastructurePrefix` property to namespace shared resources (like RabbitMQ queues or Redis keys) if necessary, avoiding collisions.
    - **Persistence**: Dedicated connection strings for each module (`Identity`, `Menu`, `Reservations`, `Orders`, `ShoppingCart`, `Payments`, `Worker`) to allow granular control over database topology (e.g., all modules in one DB vs. separate DBs).

### Consequences

#### Positive

- **POS-001**: **Deployment Flexibility**: Spawning a new restaurant instance only requires updating the configuration values (e.g., via `docker-compose` environment variables) without code changes.
- **POS-002**: **Isolation**: By facilitating separate connection strings per module and infrastructure prefixes, we ensure that while the code is shared, the data and runtime resources can be completely isolated per restaurant.
- **POS-003**: **Type Safety**: The provider wrapper ensures that configuration keys (`RestaurantConfiguration:Name`, etc.) are defined in one place, reducing "magic string" errors throughout the codebase.
- **POS-004**: **Simplicity**: Avoids the complexity of logical multi-tenancy (TenantId columns everywhere) in favor of physical isolation (Instance per Tenant), which is often safer and easier to manage for this scale.

#### Negative

- **NEG-001**: **Configuration Overhead**: Ops teams must ensure all connection strings and required fields are correctly populated for every new deployment; missing values might cause startup failures.
- **NEG-002**: **Resource Cost**: Running a separate instance (and potentially separate database servers/containers) for every small restaurant might be more resource-intensive than a multi-tenant single-instance approach.

### Alternatives Considered

#### Logical Multi-Tenancy

- **ALT-001**: **Description**: A single application instance serving multiple restaurants, distinguishing data via a `TenantId` column in every table and a global query filter.
- **ALT-001**: **Rejection Reason**: dramatically increases code complexity/security risks (data leaks) and makes database performance tuning harder. Physical isolation per restaurant is preferred for this phase.

#### Hardcoded Configuration

- **ALT-002**: **Description**: Embedding connection strings or names directly in code.
- **ALT-002**: **Rejection Reason**: Prevents CI/CD and precludes running multiple instances.
