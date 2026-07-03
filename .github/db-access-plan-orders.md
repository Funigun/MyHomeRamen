# DB Access Refactor Plan — Orders

## Goal
Migrate Orders persistence to the repository/unit-of-work pattern so feature and domain code stop depending directly on EF Core abstractions.

## Scope
- Refactor Orders module persistence around `IOrdersUnitOfWork` and aggregate-specific repositories/queries/specifications.
- Cover the concrete aggregates currently exposed by `IOrdersDbContext`: `Order`, `Product`, `Ingredient`, `Payment`, `User`, `Role`, and `Permission`.
- Preserve current transaction behavior, schema, and DI registration while moving access logic behind abstractions.

## Planned work
1. Inventory current Orders access
   - Review `IOrdersDbContext`, `OrdersDbContext`, and any existing query helpers or extension methods used by Orders features.
   - Identify which aggregates already have query/specification patterns and which still rely on ad-hoc EF Core access.

2. Define module-level unit of work
   - Add `IOrdersUnitOfWork` as the transaction boundary for the Orders module.
   - Make `OrdersDbContext` the concrete implementation and expose it through DI.

3. Define aggregate contracts
   - For each Orders aggregate, add or complete:
     - `I{Aggregate}Repository`
     - `I{Aggregate}Query`
     - `I{Aggregate}Specification`
   - Keep query interfaces read-focused and `AsNoTracking`-friendly.
   - Keep specification interfaces write-capable for persistence operations.

4. Implement persistence classes
   - Split implementation using the partial-class pattern:
     - `OrdersDbContext` for unit-of-work responsibilities
     - aggregate-specific repository/query/specification files under `MyHomeRamen.Persistance/Orders/...`
   - Keep EF Core configuration and converters in place while moving repository logic out of the context.

5. Move query and specification logic
   - Move Orders-specific extension methods into aggregate query/specification interfaces.
   - Replace direct EF Core usage in Orders feature code with repository/query abstractions.

6. Update dependency injection
   - Register `IOrdersUnitOfWork` and aggregate repositories from the Orders persistence implementation.
   - Align registrations with the current `AddOrdersPersistance` setup.

7. Update dependent layers
   - Update integration tests to depend on the new interfaces and implementations.
   - Update workers that use Orders persistence to use the new abstraction layer.

## Acceptance criteria
- Orders features use repository/query/specification abstractions instead of direct EF Core coupling.
- Orders persistence remains transactional through `IOrdersUnitOfWork`.
- Integration tests and workers switch to the new interfaces without behavior changes.
- Orders domain and feature layers no longer depend on EF Core abstractions for data access.

## Order of execution
1. Inventory and contract definition
2. Persistence implementation split
3. Feature layer migration
4. Test and worker migration
5. Verification and cleanup
