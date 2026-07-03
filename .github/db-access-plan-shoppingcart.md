# DB Access Refactor Plan — ShoppingCart

## Goal
Migrate ShoppingCart persistence to the repository/unit-of-work pattern so basket and checkout flows depend on module abstractions rather than direct EF Core access.

## Scope
- Refactor the ShoppingCart module around `IShoppingCartUnitOfWork` and aggregate-specific repository/query/specification contracts.
- Cover the aggregates currently exposed by `IShoppingCartDbContext`: `Basket`, `BasketItem`, `Product`, `Ingredient`, `ShippingDetails`, `PaymentDetails`, plus the shared identity-style `User`, `Role`, and `Permission` aggregates.
- Preserve current behavior, schema, and DI registration while moving data access behind abstractions.

## Planned work
1. Inventory current ShoppingCart access
   - Review `IShoppingCartDbContext`, `ShoppingCartDbContext`, and any basket-related query helpers or extension methods.
   - Identify which aggregates already have reusable query/specification patterns and which still rely on direct EF Core access.

2. Define module-level unit of work
   - Add `IShoppingCartUnitOfWork` as the transaction boundary for ShoppingCart persistence.
   - Make `ShoppingCartDbContext` the concrete implementation and expose it through DI.

3. Define aggregate contracts
   - For each ShoppingCart aggregate, add or complete:
     - `I{Aggregate}Repository`
     - `I{Aggregate}Query`
     - `I{Aggregate}Specification`
   - Keep query interfaces read-focused and `AsNoTracking`-friendly.
   - Keep specification interfaces write-capable for persistence operations.

4. Implement persistence classes
   - Split implementation with the partial-class pattern:
     - `ShoppingCartDbContext` for unit-of-work responsibilities
     - aggregate-specific repository/query/specification files under `MyHomeRamen.Persistance/ShoppingCart/...`
   - Keep EF Core configuration and converters intact while moving repository logic out of the context.

5. Move query and specification logic
   - Move ShoppingCart-specific extension methods into aggregate query/specification interfaces.
   - Replace direct EF Core usage in ShoppingCart feature code with repository/query abstractions.

6. Update dependency injection
   - Register `IShoppingCartUnitOfWork` and aggregate repositories from the ShoppingCart persistence implementation.
   - Align registrations with the current `AddBasketPersistance` setup.

7. Update dependent layers
   - Update integration tests to depend on the new interfaces and implementations.
   - Update workers and message handlers that use ShoppingCart persistence to use the new abstraction layer.

## Acceptance criteria
- ShoppingCart features use repository/query/specification abstractions instead of direct EF Core coupling.
- ShoppingCart persistence remains transactional through `IShoppingCartUnitOfWork`.
- Integration tests and workers switch to the new interfaces without behavior changes.
- ShoppingCart domain and feature layers no longer depend on EF Core abstractions for data access.

## Order of execution
1. Inventory and contract definition
2. Persistence implementation split
3. Feature layer migration
4. Test and worker migration
5. Verification and cleanup
