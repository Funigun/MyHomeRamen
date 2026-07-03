# DB Access Refactor Plan — Payments

## Goal
Migrate Payments persistence to the repository/unit-of-work pattern so feature code can depend on module-level abstractions instead of EF Core entry points.

## Scope
- Refactor the Payments module around `IPaymentsUnitOfWork` and aggregate-specific repository/query/specification contracts.
- Cover the aggregates currently exposed by `IPaymentsDbContext`: `Order`, `PaymentMethod`, `PaymentGateway`, `PaymentChannel`, plus the shared identity-style `User`, `Role`, and `Permission` aggregates.
- Preserve current behavior, schema, and DI registration while moving data access behind abstractions.

## Planned work
1. Inventory current Payments access
   - Review `IPaymentsDbContext`, `PaymentsDbContext`, and any existing query helpers or extension methods used by Payments features.
   - Identify which aggregates already have reusable query/specification patterns and which still rely on direct EF Core access.

2. Define module-level unit of work
   - Add `IPaymentsUnitOfWork` as the transaction boundary for Payments persistence.
   - Make `PaymentsDbContext` the concrete implementation and expose it through DI.

3. Define aggregate contracts
   - For each Payments aggregate, add or complete:
     - `I{Aggregate}Repository`
     - `I{Aggregate}Query`
     - `I{Aggregate}Specification`
   - Keep query interfaces read-focused and `AsNoTracking`-friendly.
   - Keep specification interfaces write-capable for persistence operations.

4. Implement persistence classes
   - Split implementation with the partial-class pattern:
     - `PaymentsDbContext` for unit-of-work responsibilities
     - aggregate-specific repository/query/specification files under `MyHomeRamen.Persistance/Payments/...`
   - Keep EF Core configuration and converters intact while moving repository logic out of the context.

5. Move query and specification logic
   - Move Payments-specific extension methods into aggregate query/specification interfaces.
   - Replace direct EF Core usage in Payments feature code with repository/query abstractions.

6. Update dependency injection
   - Register `IPaymentsUnitOfWork` and aggregate repositories from the Payments persistence implementation.
   - Align registrations with the current `AddPaymentsPersistance` setup.

7. Update dependent layers
   - Update integration tests to depend on the new interfaces and implementations.
   - Update workers that use Payments persistence to use the new abstraction layer.

## Acceptance criteria
- Payments features use repository/query/specification abstractions instead of direct EF Core coupling.
- Payments persistence remains transactional through `IPaymentsUnitOfWork`.
- Integration tests and workers switch to the new interfaces without behavior changes.
- Payments domain and feature layers no longer depend on EF Core abstractions for data access.

## Order of execution
1. Inventory and contract definition
2. Persistence implementation split
3. Feature layer migration
4. Test and worker migration
5. Verification and cleanup
