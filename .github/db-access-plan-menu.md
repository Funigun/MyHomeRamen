# DB Access Refactor Plan — Menu

## Goal
Migrate Menu data access to the repository/unit-of-work pattern already used by the Menu features layer, removing direct EF Core coupling from domain and feature code where possible.

## Scope
- Complete the Menu refactor for remaining aggregates and query/specification surfaces.
- Preserve current behavior, schemas, and DI registrations.
- Keep changes aligned with existing Menu abstractions and persistence patterns.

## Planned work
1. Inventory Menu aggregates and current EF Core usage
   - Identify every aggregate currently accessed through `MenuDbContext` or extension methods.
   - Confirm which aggregates already have query/specification/repository interfaces and which still rely on shared extension methods.

2. Define module-level contract
   - Add or finalize `IMenuUnitOfWork` as the transaction boundary for Menu persistence.
   - Ensure `MenuDbContext` exposes it through DI and remains the concrete implementation.

3. Define aggregate contracts
   - For each Menu aggregate, create or complete:
     - `I{Aggregate}Repository`
     - `I{Aggregate}Query`
     - `I{Aggregate}Specification`
   - Keep query APIs `AsNoTracking`-friendly and specification APIs write-capable.

4. Implement persistence classes
   - Split implementation into partial classes under persistence:
     - `MenuDbContext` for unit-of-work responsibilities
     - `Categories/CategoryRepository.cs`
     - `Categories/CategoryQuery.cs`
     - `Categories/CategorySpecification.cs`
   - Apply same pattern for any other Menu aggregates still missing it.

5. Move query/specification logic
   - Move existing extension methods from the feature layer into aggregate query/specification interfaces.
   - Replace direct EF Core access in feature handlers/queries with repository/query interfaces.

6. Update dependency injection
   - Register `IMenuUnitOfWork` and aggregate repositories from `MenuDbContext` or the new partial implementations.
   - Keep scope consistent with current persistence setup.

7. Update tests and workers
   - Adjust integration tests to depend on the new interfaces and implementations.
   - Update workers that access Menu persistence to use the new abstraction layer.

## Acceptance criteria
- Menu features use repository/query/specification abstractions instead of direct EF Core extension coupling.
- Menu persistence remains transactional through `IMenuUnitOfWork`.
- Existing integration tests pass after switching to the new interfaces.
- No new EF Core dependency is introduced in Menu domain or feature code.

## Order of execution
1. Inventory and contract definition
2. Persistence implementation split
3. Feature layer migration
4. Test and worker migration
5. Verification and cleanup
