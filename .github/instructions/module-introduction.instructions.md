---
description : Guidelines and checklist for introducing a new module into the MyHomeRamen modular monolith architecture
applyTo: '*'
---

# Module Introduction Guide

This document outlines the required steps to introduce a new module into the MyHomeRamen application, ensuring consistency across all layers of the modular monolith.

## 1. Keycloak Configuration

Before writing code that relies on authorization, configure the module's scopes and roles:
- **Keycloak Admin Panel:** 
  - Create a new client scope for the module (e.g., `new_module`).
  - Create module-specific roles (e.g., `new_module_customer`, `new_module_employee`, `new_module_admin`).
  - Link the roles to the module scope.
- **Code Generation:** Update `MyHomeRamen.Infrastructure\Keycloak\Constants\KeycloakRoleConstants.cs` to include the new roles. Ensure you add them to the `AllRoles` list and properly map them in the `RoleMappings` and `CustomerRoles` as appropriate.

## 2. Aspire Host Integration

Integrate the new module into the Aspire orchestration and shared AppHost configuration:
- **User Secrets:** Create user secrets for the Database admin user specific to the new module.
- **Constants:** Add the module name constant in `MyHomeRamen.AppHost\Configurations\Common\ConfigurationConstants.cs` (e.g., `internal const string NewModuleName = "NewModule";`).
- **Registration Extensions:** Update `MyHomeRamen.AppHost\InfrastructureConfiguration\ProjectRegistrationExtensions.cs` to include the new module name in the `requiredModules` collections for `AddApiService`, `AddDbinitializer`, and `AddMessagesHandlerWorker` where applicable.

## 3. Domain Integration

Establish the core business rules and abstractions for the module following `domain.instructions.md`:
- **Folder Structure:** Create the module structure in `MyHomeRamen.Domain/{Module}`.
- **Database Abstraction:** Create an interface `I{Module}DbContext` that inherits from `MyHomeRamen.Api.Common.Domain.IBaseDbContext`.
- **Domain Models:** Create the following module-specific scoped models (ensuring they use strongly-typed IDs):
  - `User` 
  - `Role`
  - `Permission`
- **Relationships:** Design the domain to reflect that a `User` has many `Role`s and `Permission`s, and a `Role` has many `Permission`s.
- **Constants:** Create `RoleConstants` and `PermissionConstants` corresponding to the respective aggregate.

## 4. Persistence Integration

Implement the database layer for the module following `persistence.instructions.md`:
- **Folder Structure:** Create the module structure in `MyHomeRamen.Persistence/{Module}`.
- **ID Converters:** Implement specific EF Core value converters for the strongly-typed IDs introduced in the Domain.
- **DbContext Implementation:** Create `{Module}DbContext` implementing the `I{Module}DbContext` defined in the domain layer. Ensure it inherits from the base context implementations.
- **Configurations:** Add `IEntityTypeConfiguration` implementations for all domain aggregates and entities.
- **Schema Mapping:** Apply the default schema in the `OnModelCreating` method using `modelBuilder.HasDefaultSchema("{module}");`.

## 5. `MyHomeRamen.Api` Project Integration

Wire up the new module in the main REST API layer:
- **Folder:** Create a new `{Module}` folder in the `MyHomeRamen.Api` project.
- **Dependency Injection:** Create a `DependencyInjection.cs` file with an extension method (e.g., `Add{Module}Module`) configured similarly to other modules. Register application services, handlers, and the persistence implementation.

## 6. Database Initializer Worker Integration

Ensure the new module's database is automatically provisioned and migrated on startup:
- **Update Job:** In `MyHomeRamen.Worker.DatabaseInitializer\DbInitializerJob.cs`, inject the new `I{Module}DbContext`.
- **Add to Configuration:** Add the injected context to the `dbContexts` dictionary tracking system, utilizing `DatabaseUserConfig.Create("{Module}", configuration)`.

## 7. Messages Handler Worker Integration

Set up integration event listening for cross-module communication (e.g., user replication):
- **Folder:** Create a `{Module}` folder inside the `MyHomeRamen.Worker.MessagesHandler` project.
- **Event Handler:** Create a handler class (e.g., `{Module}UserRegisteredHandler`) that implements `MyHomeRamen.Worker.Common.IIntegrationEventHandler<MyHomeRamen.Common.Contracts.Messaging.UserRegisteredIntegrationEvent>`.
- **Implementation:** Ensure this handler correctly processes the sync/creation of localized user records in the new module's specific database context when a global user registers.

## 8. Blazor Frontend Integration

- **Authorization:** Add scope for new module in `AuthenticationDependencyInjection` 