# DB Access Refactor Plan — Users

## Goal
Migrate Users persistence to the repository/unit-of-work pattern so identity and profile flows depend on module abstractions rather than direct EF Core access.

## Scope
- Refactor the Users module around `IUsersUnitOfWork` and aggregate-specific repository/query/specification contracts.
- Cover the aggregates currently exposed by `IUsersDbContext`: `User`, `Role`, and `Address`.
- Preserve current behavior, schema, and DI registration while moving data access behind abstractions.

## Planned work
1. Inventory current Users access
   - Review `IUsersDbContext`, `UsersDbContext`, and any user-related query helpers or extension methods.
   - Identify which aggregates already have reusable query/specification patterns and which still rely on direct EF Core access.

2. Define module-level unit of work
   - Add `IUsersUnitOfWork` as the transaction boundary for Users persistence.
   - Make `UsersDbContext` the concrete implementation and expose it through DI.

3. Define aggregate contracts
   - For each Users aggregate, add or complete:
     - `I{Aggregate}Repository`
     - `I{Aggregate}Query`
     - `I{Aggregate}Specification`
   - Keep query interfaces read-focused and `AsNoTracking`-friendly.
   - Keep specification interfaces write-capable for persistence operations.

4. Implement persistence classes
   - Split implementation with the partial-class pattern:
     - `UsersDbContext` for unit-of-work responsibilities
     - aggregate-specific repository/query/specification files under `MyHomeRamen.Persistance/Users/...`
   - Keep EF Core configuration and converters intact while moving repository logic out of the context.

5. Move query and specification logic
   - Move Users-specific extension methods into aggregate query/specification interfaces.
   - Replace direct EF Core usage in Users feature code with repository/query abstractions.

6. Update dependency injection
   - Register `IUsersUnitOfWork` and aggregate repositories from the Users persistence implementation.
   - Align registrations with the current `AddIdentityPersistance` setup.

7. Update dependent layers
   - Update integration tests to depend on the new interfaces and implementations.
   - Update workers and message handlers that use Users persistence to use the new abstraction layer.

## Acceptance criteria
- Users features use repository/query/specification abstractions instead of direct EF Core coupling.
- Users persistence remains transactional through `IUsersUnitOfWork`.
- Integration tests and workers switch to the new interfaces without behavior changes.
- Users domain and feature layers no longer depend on EF Core abstractions for data access.

## Order of execution
1. Inventory and contract definition
2. Persistence implementation split
3. Feature layer migration
4. Test and worker migration
5. Verification and cleanup
