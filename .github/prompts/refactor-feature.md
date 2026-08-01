# Refactor Single Feature

Refactor the `[FeatureName]` feature in `MyHomeRamen.Features.[Module]` to follow vertical-slice organization and remove dependency on `MyHomeRamen.Common.Contracts` for request/response models.

## Context

- Current feature entry point: `[path to endpoint/handler]`
- Current repository method: `[path to repository method]`
- Target module: `[Menu/Ordering/etc.]`

## Steps

### 1. Request model

- If the endpoint uses query parameters only, create a new request record in `Models/{FeatureName}Request.cs`.
- If a request model exists in `Common.Contracts`, create duplicate of that and duplicates of DTOs (if any) in `Models/{FeatureName}Request.cs`.
- If validator exists in .

### 2. DTO

- Create `Models/Dto.cs` with the shape needed by the handler from persistence.
- This DTO bridges DAO (persistence) and Response (API contract).

### 3. Query + Handler

- Merge query/command and handler into a single `[FeatureName].cs` file.
- Query/Command record lives as a nested type inside the static class.
- Handler implements `IQueryHandler<TQuery, TResult>` or `ICommandHandler<TCommand, TResult>`.
- Handler builds `DbQueryOptions` via a strongly-typed options record if parameters are needed for the cache key or projection.

### 4. Repository adjustment

- Locate the repository method in `MyHomeRamen.Persistence.[Module]`.
- Update its signature to accept the strongly-typed `DbQueryOptions` record.
- Ensure projection, filtering, and ordering stay in `DbQueryOptions`; joins/`Include` logic stays in the repository.
- Apply authorization/policy if present.

### 5. Cache policy

- If the feature should be cached, define `CachePolicy` in the repository method or handler.
- Use `CachePolicy.LocalCache<TModule>` / `DistributedCache<TModule>` / `HybridCache<TModule>`.
- Cache key must include all variable parameters (e.g., `"CategoryByTypeDto:{categoryType}"`).
- Cache tags should include entity tags if the dynamic tags overload is needed (e.g., `product:{id}`).
- Document expiration time and invalidation strategy in a comment.

### 6. Response

- Create `Models/Response.cs`.
- Map from DTO to Response in the handler.
- Do not expose domain entities or DAOs in the response.

### 7. Endpoint

- Update the endpoint to bind to the new request model.
- Map the endpoint to the handler via the existing pipeline.
- Keep the endpoint file separate unless trivial.

### 8. Validation

- Move/adapt validators to the feature folder or module `Common/Validators`.
- Ensure primitive validators (e.g., `CategoryNameValidator`) live in module `Common/Validators` if shared.

### 9. Blazor frontend

- Find usages of old `Common.Contracts` request/response types in Blazor.
- Update service calls and DTOs to match the new Response shape.
- Update page/component binding if property names changed.

### 10. Cleanup

- Remove old request/response types from `Common.Contracts`.
- Verify no integration events were moved (keep those in `Common.Contracts`).
- Run build and targeted tests.

## Constraints

- Do not leak EF Core into `Features`.
- Keep domain entities in the `Domain` project.
- Follow existing test naming: `{MethodName}_Should{Behavior}_For{Condition}` if adding tests.
- Use primary constructors.
- Do not use `var`.
