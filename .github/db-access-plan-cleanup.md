# DB Access Refactor Plan — Cleanup

## Goal
Remove legacy EF Core-based database access abstractions after every module has been migrated to the repository/unit-of-work pattern.

## Scope
- Remove old DbContext-specific abstractions and any remaining direct EF Core coupling from domain and feature layers.
- Leave only the new repository/query/specification abstractions and persistence implementations in place.
- Preserve behavior, build health, and architecture constraints during the cleanup phase.

## Planned work
1. Verify module migration completion
   - Confirm every module in the refactor scope has a unit-of-work contract, aggregate repositories/queries/specifications, and persistence implementation.
   - Confirm features, tests, and workers now depend on the new interfaces instead of the legacy abstractions.

2. Remove legacy abstractions
   - Remove old module DbContext interfaces such as `IMenuDbContext`, `IOrdersDbContext`, `IPaymentsDbContext`, `IShoppingCartDbContext`, `IUsersDbContext`, and `IReservationsDbContext` once all callers have moved to the new contracts.
   - Remove obsolete repository helper abstractions that were only needed during transition.

3. Remove EF Core coupling from domain and features
   - Delete any remaining references to EF Core types from domain-layer abstractions and feature-layer code.
   - Ensure domain entities and feature handlers only depend on domain-level contracts and application abstractions.

4. Simplify persistence registrations
   - Keep DI registrations focused on the new unit-of-work and repository interfaces.
   - Remove transitional registrations that exist only to wire legacy abstractions.

5. Run architecture and regression validation
   - Run architecture tests and targeted integration tests to ensure the cleanup did not break module boundaries.
   - Verify the solution still builds and persistence still works end to end.

## Acceptance criteria
- No legacy module DbContext interfaces remain in the solution.
- Domain and feature layers no longer reference EF Core-specific database abstractions.
- New repository/query/specification abstractions are the only data access contracts used by feature code.
- Architecture tests and relevant integration tests pass after cleanup.

## Order of execution
1. Final migration verification
2. Legacy abstraction removal
3. Coupling cleanup
4. DI simplification
5. Validation and stabilization
