---
name: domain-preparation
description: Prepare domain model aggregates, entities, etc including constants, errors, validators and DB configuration according to project standards.
---

# Domain Preparation

Prepare complete domain and persistence foundations from feature requirements.

## Scope

- Domain aggregates, entities, value objects, constants, errors, and validators, strongly typed IDs
- Static constructor method using dedicated `Validator`
- Feature-layer abstractions (DbContext, repository, query, loader interfaces)
- Persistance implementation (Converters, DB Configurations, feature-layer abstractions, DbContext, DbContext factory and DB initializer for worker)
- Persistance is either update existing (work on existing module) or creating new implementations (starting new module)
- Aspire App-host update

## Implementation Order

### 1. Strongly Typed Id for each Aggregate/Entity
public record struct {AggregateName}Id(Guid Value) : IEntityId with explicit operators and ToString() override

### 2. Entities and Aggregates
- define constants and errors for each entity/aggregate
- define model shape with properties, relationships and navigation properties
- define validator
- define static constructor method that use dedicated `Validator`
- define private empty constructor for EF
- do not implement any behavior

### 3. Feature-layer abstractions
- define `I{Aggregate}Query` and `I{Aggregate}Loader` interfaces without any methods
- define `I{Aggregate}Repository` interface with methods 2 methods: `I{Aggregate}Loader Load()` and `I{Aggregate}Query Query()`
- define `I{Module}DbContext` interface with `I{Aggregate}Repository`

### 4. Persistance implementation
- create `ValueConverter` for each strongly typed Id added in step 1
- create `DbConfiguration` for each entity/aggregate added in step 2
- create `{Module}DbContext` implementation with `DbSet` for each entity/aggregate added in step 2 and implement `I{Module}DbContext`
- create `{Mopdule}DbContextFactory`
- create query, loader and repository implementations for each entity/aggregate added in step 2
- define `ICache{Module}` interface that implements `ICacheModule`
- update Dependency Injection

### 5. DB Initializer Worker
- update `DbInitializerJob` to use new `I{Module}DbContext` in migration/seeding loop

### 6. Aspire App-host update
- update `ConfigurationConstants` with new module name
- update `ProjectRegistrationExtensions` to forward variables of new module to Api/Worker

### 7. User-Secrets
- Notify user that `Aspire-App` host user-secrets and `Api` projects require user-secrets updates for new module 