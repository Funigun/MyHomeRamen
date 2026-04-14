# 3. Cache Policy Handling — Analysis

## Current State

| Component | Location | Status |
|---|---|---|
| `ICachePolicy<TRequest, TResponse>` | `MyHomeRamen.Api.Common` | ✅ Defined |
| `ICacheService` | `MyHomeRamen.Api.Common` | ✅ Defined |
| `CacheService` (HybridCache) | `MyHomeRamen.Infrastructure` | ✅ Implemented |
| `KeycloakAdminTokenCachePolicy` | `MyHomeRamen.Infrastructure` | ✅ Used (infrastructure-level) |
| Feature-level cache policies | — | ❌ Not yet created |
| Cache integration in handlers | — | ❌ Not yet implemented |

The `ICacheService.GetOrSetAsync` API is already cache-aside (factory pattern):
```csharp
cache.GetOrSetAsync(policy, async ct => await /* db query */, cancellationToken);
```

The question is: **where does the `GetOrSetAsync` call live?**

---

## Three Approaches

### Approach A — Cache in Handler (explicit, per-feature)

Each handler that needs caching gets `ICacheService` injected and wraps its query.

**Handler example (GetCategoriesByType):**
```csharp
public sealed class GetCategoriesByTypeHandler(
    IMenuDbContext dbContext,
    ICacheService cacheService)
    : IRequestHandler<GetCategoriesByTypeRequest, IEnumerable<GetCategoriesByTypeResponse>>
{
    public async Task<IEnumerable<GetCategoriesByTypeResponse>> Handle(
        GetCategoriesByTypeRequest request, CancellationToken ct)
    {
        var policy = new GetCategoriesByTypeCachePolicy(request.CategoryType);

        return await cacheService.GetOrSetAsync(policy, async ct2 =>
        {
            CategoryType categoryType = (CategoryType)request.CategoryType;
            return await dbContext.Categories
                .ForCategoryType(categoryType)
                .Select(c => c.ToResponse())
                .ToListAsync(ct2);
        }, ct);
    }
}
```

**Cache policy (lives in the feature's `Policies/` folder):**
```csharp
public sealed class GetCategoriesByTypeCachePolicy(int categoryType)
    : ICachePolicy<GetCategoriesByTypeRequest, IEnumerable<GetCategoriesByTypeResponse>>
{
    public string Key => $"categories-by-type-{categoryType}";
    public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(30);
    public TimeSpan? LocalExpirationTime => TimeSpan.FromMinutes(5);
}
```

**Invalidation (in CreateCategoryHandler / DeleteCategoryHandler / UpdateCategoriesOrderHandler):**
```csharp
await cacheService.RemoveAsync(
    new GetCategoriesByTypeCachePolicy(request.CategoryType), ct);
```

**Pros:**
- Zero infrastructure changes — uses existing `ICacheService` as-is.
- Completely explicit: reader sees exactly what is cached and when.
- Each feature controls its own cache key, TTL, and invalidation.
- Easy to test: mock `ICacheService` in unit tests.
- Fits perfectly with existing vertical-slice architecture.

**Cons:**
- Repetitive boilerplate if many handlers need caching.
- Handler has two responsibilities (orchestration + cache management).

---

### Approach B — Pipeline Behavior / Decorator (cross-cutting)

Add a generic `CachingRequestHandlerDecorator` that wraps any `IRequestHandler<TRequest, TResponse>` when a cache policy exists.

**Decorator:**
```csharp
public sealed class CachingRequestHandlerDecorator<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> inner,
    ICacheService cacheService,
    ICachePolicy<TRequest, TResponse> cachePolicy)
    : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken ct)
    {
        return await cacheService.GetOrSetAsync(cachePolicy, async ct2 =>
            await inner.Handle(request, ct2), ct);
    }
}
```

**DI registration (using Scrutor or manual):**
```csharp
// For each feature that has a cache policy:
services.Decorate<
    IRequestHandler<GetCategoriesByTypeRequest, IEnumerable<GetCategoriesByTypeResponse>>,
    CachingRequestHandlerDecorator<GetCategoriesByTypeRequest, IEnumerable<GetCategoriesByTypeResponse>>>();
```

Or auto-register all handlers that have a matching `ICachePolicy<,>` in the container.

**Cache policy remains the same** as Approach A, but is registered in DI rather than created in the handler.

**Invalidation** still happens explicitly in mutation handlers — the decorator only handles reads.

**Pros:**
- Handlers stay clean (single responsibility).
- Cache logic is centralized in one decorator.
- Adding caching to a feature = create a policy class + register the decorator.

**Cons:**
- More complex DI wiring (especially without MediatR's pipeline).
- `ICachePolicy` needs the request instance to build dynamic keys (e.g., `categoryType`), so the policy must be a factory or accept request in `Key` — requires changing `ICachePolicy<TRequest, TResponse>`.
- Cache invalidation still requires explicit calls in mutation handlers.
- Harder to debug — behavior is implicit.

**ICachePolicy change needed:**
```csharp
public interface ICachePolicy<TRequest, TResponse>
{
    string GetKey(TRequest request);  // instead of just "Key"
    TimeSpan? ExpirationTime { get; }
    TimeSpan? LocalExpirationTime { get; }
}
```

---

### Approach C — Cache-Aware Specification Builder Terminal

Add a terminal method to the `SpecificationBuilder<T>` (from file `02`) that accepts cache parameters:

```csharp
public async Task<List<TResult>> ProjectToListCachedAsync<TRequest, TResult>(
    Expression<Func<TEntity, TResult>> projection,
    ICacheService cacheService,
    ICachePolicy<TRequest, List<TResult>> policy,
    CancellationToken ct = default)
{
    return await cacheService.GetOrSetAsync(policy, async ct2 =>
        await _query.Select(projection).ToListAsync(ct2), ct);
}
```

**Handler example:**
```csharp
return await dbContext.Categories
    .Specify()
    .Filter(new CategoryByTypeSpec(categoryType))
    .OrderBy(c => c.SortOrder)
    .ProjectToListCachedAsync(
        c => c.ToResponse(),
        cacheService,
        new GetCategoriesByTypeCachePolicy(request.CategoryType),
        ct);
```

**Pros:**
- Cache is opt-in at the query level — just swap `ProjectToListAsync` → `ProjectToListCachedAsync`.
- Natural fit with the specification builder concept.
- Handler stays relatively clean.

**Cons:**
- Specification builder gains `ICacheService` dependency (even if only as parameter).
- Only works for list projections — need additional terminals for single-entity, count, etc.
- Tightly couples specification execution with cache strategy.

---

## Comparison Matrix

| Criterion | A: Handler | B: Decorator | C: Spec Terminal |
|---|---|---|---|
| Implementation effort | Low | Medium-High | Medium |
| Handler cleanliness | Medium | High | Medium-High |
| Explicit control | High | Low | High |
| DI complexity | None | High | None |
| ICachePolicy changes | None | Yes (GetKey) | None |
| Fits current architecture | ✅ Perfect | ⚠️ Needs pipeline | ⚠️ Needs spec builder first |
| Testability | Easy | Easy | Easy |
| Invalidation story | Explicit | Explicit | Explicit |
| Incremental adoption | Feature-by-feature | All-or-nothing setup | Needs spec builder first |

---

## Invalidation Strategy (applies to all approaches)

Regardless of where caching lives, mutation handlers must invalidate:

| Mutation | Invalidation |
|---|---|
| `CreateCategory` | Remove `categories-by-type-{type}` |
| `DeleteCategory` | Remove `categories-by-type-{type}` |
| `UpdateCategoriesOrder` | Remove `categories-by-type-{type}` |
| `CreateProduct` | Remove `products-by-category-{catId}` for each category |
| `UpdateProduct` | Remove affected category caches |

For simple cases, explicit `cacheService.RemoveAsync(…)` in the mutation handler is cleanest. For complex cross-feature invalidation, consider domain events.

---

## Recommendation

**Start with Approach A (Cache in Handler)** — it requires zero infrastructure changes, fits the current vertical-slice architecture perfectly, and gives full explicit control. This is the pragmatic starting point.

**Consider evolving to Approach B (Decorator)** later if:
- More than ~10 handlers need caching.
- The `ICachePolicy` interface is refactored to accept the request for dynamic key generation.
- A pipeline/decorator infrastructure is added to the custom mediator.

**Approach C** is viable only after the specification builder (file `02`) is implemented and battle-tested.
