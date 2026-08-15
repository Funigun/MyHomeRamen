---
title: "ADR-0005: Repository Strategy"
status: "Accepted"
date: "2026-07-29"
authors: "Funigun"
tags: ["architecture", "decision", "repository", "data-access", "dao"]
---

### Status

**Accepted**

### Context

My Home Ramen uses a modular monolith architecture. Each module owns its aggregates, persistence context and features. Data access must satisfy several constraints:

- Reads should fetch only required columns from the database (EF projection).
- Write operations must load full aggregates so domain invariants can be enforced.
- Query logic must be reusable across API handlers and background workers without duplicating mapping code.
- Persistence primitives (`DbSet<T>`, `IQueryable<T>`) must not leak outside the persistence implementation.
- The API contract shape (request/response DTOs in `MyHomeRamen.Common.Contracts`) must not be coupled to the database projection shape.

Existing code mixed these responsibilities: some handlers load full entities and map in memory, others pass `Expression<Func<Entity, Dto>>` into query methods. We need a single, consistent pattern.

This decision complements **ADR-0006: Cache Strategy**, which uses repository decorators and tag-based invalidation. DAOs are the "query projections" that the cache strategy caches.

### Decision

We will use a **Repository + Query + Specification** pattern, complemented by **Data Access Objects (DAOs)** for read models.

#### 1. Repository (`IRepository<TEntity, TId>`)

`MyHomeRamen.Features/Common/Repository/IRepository.cs` defines write primitives shared by every aggregate:

- `Add` / `AddRange`
- `Delete`
- `Exists`
- `ExecuteDelete`
- `ExecuteUpdate`
- `Count`

These operate on full domain entities. Concrete repositories inherit `BaseRepository<TEntity, TId>` in `MyHomeRamen.Persistence/Common/BaseRepository.cs`, which also provides protected `QueryList`, `QueryFirst`, `QueryPaged` helpers.

#### 2. Aggregate repository (`I{Aggregate}Repository`)

Each aggregate exposes a single repository interface that returns specialized read and write facades:

```csharp
public interface IProductRepository : IRepository<Product, ProductId>
{
    IProductQuery Query();
    IProductSpecification Specification();
}
```

The concrete partial repository (`MyHomeRamen.Persistence/Menu/ProductRepository.cs`) implements all three contracts.

#### 3. Query interface (`I{Aggregate}Query`)

`I{Aggregate}Query` lives in `MyHomeRamen.Features/{Module}/Features/{Aggregate}/Common`. It declares module-specific read methods. Example: `MyHomeRamen.Features/Menu/Features/Products/Common/IProductQuery.cs`.

Query methods return **DAOs**, not domain entities and not API response DTOs. They use `DbQueryOptions<TEntity>` and `DbPagedQueryOptions<TEntity>` internally to build filtered, ordered and paged queries.

#### 4. Specification interface (`I{Aggregate}Specification`)

`I{Aggregate}Specification` defines methods that load full aggregates for command handlers. Example: `MyHomeRamen.Features/Menu/Features/Products/Common/IProductSpecification.cs` exposes `ById` returning the full `Product` aggregate.

#### 5. DAOs (Data Access Objects)

DAOs are persistence-layer read models:

- Located in `MyHomeRamen.Persistence/{Module}/Dao/`.
- Public `sealed record` types.
- Contain only primitive or simple data shaped for the query use case.
- Mapped from entities via EF `.Select()` inside query implementations.

Example:

```csharp
// MyHomeRamen.Persistence/Menu/Dao/IngredientDao.cs
public sealed record IngredientDao(Guid Id, string Name, string Description);
```

Query methods return DAOs:

```csharp
public interface IIngredientQuery
{
    Task<PagedResult<IngredientDao>> GetForManageAsync(
        string? name,
        IEnumerable<Guid>? categoryIds,
        PageParameters page,
        OrderParameters order,
        CancellationToken cancellationToken);
}
```

#### 6. `DbQueryOptions` and `DbPagedQueryOptions`

`MyHomeRamen.Persistence/Common/DbQueryOptions.cs` and `DbPagedQueryOptions.cs` are internal persistence helpers. They encapsulate filter, ordering and pagination data. They are never exposed through `I{Aggregate}Query` interfaces.

`BaseRepository` uses them to compose `IQueryable<TEntity>` and then applies the DAO projection before materializing the query.

#### 7. Mapping flow

```text
Entity (Domain) --[EF .Select() in Persistence]--> DAO (Persistence)
DAO (Persistence) --[Mappings in Features]--> Response DTO (Common.Contracts)
```

- Persistence owns `Entity → DAO` projection. This is where EF reads only required columns.
- Features handlers own `DAO → Response` mapping. This is where config-based URLs, enrichment and response shaping happen.
- Background workers can call the same query methods and operate directly on DAOs, or map to worker-specific shapes.

### Consequences

#### Positive

- **POS-001**: **Database efficiency**: EF projects queries to DAOs, reading only the columns needed for each use case.
- **POS-002**: **Worker reuse**: Background workers reference `Persistence` and reuse the same DAO-producing queries; no mapping duplication.
- **POS-003**: **Decoupled contracts**: API response DTOs in `Common.Contracts` are independent from the database read shape.
- **POS-004**: **Clear responsibilities**: writes use full aggregates via specifications, reads use lightweight DAOs via queries.
- **POS-005**: **Encapsulated persistence primitives**: `DbQueryOptions`, `DbSet` and `IQueryable` stay inside `Persistence`.
- **POS-006**: **Cache-friendly read models**: DAOs are flat records with primitive data, making them ideal for the repository-level caching described in ADR-0006. They avoid EF Core change-tracking traps, serialization issues and circular references associated with caching raw entities.
- **POS-007**: **Transparent caching**: Module-scoped repository decorators (e.g., `Menu.CachedUserRepository`) wrap concrete repositories. Handlers keep calling `Query().{Method}(...)`; the decorator handles cache lookup, storage and tag-based invalidation.
- **POS-008**: **Consistent invalidation across entry points**: Because caching lives at the repository level, API handlers, background workers and other entry points share the same cache and invalidation rules. ADR-0006 covers automatic eviction via a `SaveChanges` interceptor, manual eviction for bulk operations and cross-module eviction via integration events.
- **POS-009**: **No audit / metadata leakage**: DAOs contain only the data needed for the use case. They do not include `AuditableEntity` fields (CreatedBy, CreatedOn, ModifiedBy, ModifiedOn) or other entity metadata that would be loaded by default, so these values cannot leak into the cache or API responses by accident.
- **POS-010**: **Workers benefit from distributed cache**: Because repository decorators cache DAOs at the data access layer, background workers calling the same query methods hit Redis instead of the database. The cache is shared across all entry points.

#### Negative

- **NEG-001**: **More types**: Each query use case may add a DAO record and a mapping method.
- **NEG-002**: **Extra mapping layer**: `Entity → DAO → Response` requires two mapping steps instead of one.
- **NEG-003**: **Feature-to-persistence DAO dependency**: `I{Aggregate}Query` interfaces in `Features` reference DAO types from `Persistence`. Acceptable because `Features` already references `Persistence`, but it couples the application abstraction to a persistence type.
- **NEG-004**: **DAO identity exposure for tags**: Tag-based invalidation from ADR-0006 requires cache entries to be tagged with underlying entity IDs. DAOs must expose the IDs of all entities that, when changed, should invalidate the cached result. For aggregate-spanning queries this may require additional identity fields in the DAO.
- **NEG-005**: **Serialization constraints on DAOs**: DAOs intended for caching must be serializable (e.g., JSON) and avoid circular references, private constructors or EF-specific types. This constrains DAO design.
- **NEG-006**: **Cache key and tag complexity**: Query methods with filters, ordering and pagination need stable, deterministic cache keys. Tags must cover every entity dependency to avoid stale data, while staying small enough to keep cache metadata overhead low.
- **NEG-007**: **Bulk-operation invalidation gap**: `ExecuteUpdate` and `ExecuteDelete` bypass the EF Core change tracker. Repository methods using these must manually trigger tag evictions, otherwise cached DAOs will become stale.

### Alternatives Considered

#### Return `Common.Contracts` DTOs directly from persistence

- **Description**: Query methods return response DTOs defined in `Common.Contracts`.
- **Rejection reason**: Couples persistence to the API contract. Any response change forces persistence changes, and workers become tied to API-specific shapes.

#### Keep mapping in `Features` and pass expressions to persistence

- **Description**: Handlers define `Expression<Func<Entity, Dto>>` and pass it into query methods.
- **Rejection reason**: Non-projectable logic (config URLs, service calls) cannot live inside EF expressions, forcing client-side evaluation or post-query enrichment. Workers that do not reference `Features` would duplicate mappings.

#### Expose `IQueryable<T>` from persistence

- **Description**: Allow handlers to compose their own LINQ queries.
- **Rejection reason**: Violates the rule that persistence primitives must not leak outside the persistence implementation. Makes testing and optimization harder.

