# DB Access Refactor Plan — Reservations

## Goal
Migrate Reservations persistence to the repository/unit-of-work pattern so booking and table flows depend on module abstractions rather than direct EF Core access.

## Scope
- Refactor the Reservations module around `IReservationsUnitOfWork` and aggregate-specific repository/query/specification contracts.
- Cover the aggregates currently exposed by `IReservationsDbContext`: `Booking`, `Table`, plus the shared identity-style `User`, `Role`, and `Permission` aggregates.
- Preserve current behavior, schema, and DI registration while moving data access behind abstractions.

## Planned work
1. Inventory current Reservations access
   - Review `IReservationsDbContext`, `ReservationsDbContext`, and any booking/table query helpers or extension methods.
   - Identify which aggregates already have reusable query/specification patterns and which still rely on direct EF Core access.

2. Define module-level unit of work
   - Add `IReservationsUnitOfWork` as the transaction boundary for Reservations persistence.
   - Make `ReservationsDbContext` the concrete implementation and expose it through DI.

3. Define aggregate contracts
   - For each Reservations aggregate, add or complete:
     - `I{Aggregate}Repository`
     - `I{Aggregate}Query`
     - `I{Aggregate}Specification`
   - Keep query interfaces read-focused and `AsNoTracking`-friendly.
   - Keep specification interfaces write-capable for persistence operations.

4. Implement persistence classes
   - Split implementation with the partial-class pattern:
     - `ReservationsDbContext` for unit-of-work responsibilities
     - aggregate-specific repository/query/specification files under `MyHomeRamen.Persistance/Reservations/...`
   - Keep EF Core configuration and converters intact while moving repository logic out of the context.

5. Move query and specification logic
   - Move Reservations-specific extension methods into aggregate query/specification interfaces.
   - Replace direct EF Core usage in Reservations feature code with repository/query abstractions.

6. Update dependency injection
   - Register `IReservationsUnitOfWork` and aggregate repositories from the Reservations persistence implementation.
   - Align registrations with the current `AddReservationsPersistance` setup.

7. Update dependent layers
   - Update integration tests to depend on the new interfaces and implementations.
   - Update workers and message handlers that use Reservations persistence to use the new abstraction layer.

## Acceptance criteria
- Reservations features use repository/query/specification abstractions instead of direct EF Core coupling.
- Reservations persistence remains transactional through `IReservationsUnitOfWork`.
- Integration tests and workers switch to the new interfaces without behavior changes.
- Reservations domain and feature layers no longer depend on EF Core abstractions for data access.

## Order of execution
1. Inventory and contract definition
2. Persistence implementation split
3. Feature layer migration
4. Test and worker migration
5. Verification and cleanup
