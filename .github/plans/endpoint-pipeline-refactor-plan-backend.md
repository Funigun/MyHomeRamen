# Plan: Endpoint Pipeline Refactor — Command/Query + Handler Decorators

## Context

Two reference endpoints are used to illustrate both patterns:
- **Command** → `CreateCategory` (POST): introduces `ICommand` / `CreateCategoryCommand` / handler decorator pipeline
- **Query** → `GetCategoriesByType` (GET): introduces `IQuery` / `GetCategoriesByTypeQuery` / handler decorator pipeline

---

## Implementation plan

---

### Step 1: Refactor ValidationFilter and AuthorizationFilter into IRequestHandler Decorators

**Goal**: replace endpoint-level `IEndpointFilter` execution with in-handler decorator execution so validation and authorization fire *after* full model binding (including route IDs) and inside the handler chain, not before it.

#### 1.1 — Introduce `ICommandHandler` and `IQueryHandler` interfaces in `MyHomeRamen.Api.Common`

Location: `MyHomeRamen.Api.Common/Endpoint/Models/`

```
ICommandHandler.cs   ← replaces IRequestHandler for mutations
IQueryHandler.cs     ← replaces IRequestHandler for reads
```

Signatures (follow primary constructor and no-`var` conventions):

```csharp
// ICommandHandler.cs
public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    Task Handle(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken);
}

// IQueryHandler.cs
public interface IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}
```

#### 1.2 — Introduce `ICommand` and `IQuery` marker interfaces in `MyHomeRamen.Api.Common`

Location: `MyHomeRamen.Api.Common/Endpoint/Models/`

```csharp
// ICommand.cs
public interface ICommand { }
public interface ICommand<TResponse> : ICommand { }

// IQuery.cs
public interface IQuery<TResponse> { }
```

> Note: `ICommand` / `IQuery` are **internal API-layer concepts** — they are NOT shared with `MyHomeRamen.Common.Contracts`. Request/Response records move to Common.Contracts (Step 2); Command/Query objects live only in the API layer.

#### 1.3 — Create `ValidationHandlerDecorator<TCommand, TResponse>` in `MyHomeRamen.Api.Common`

Location: `MyHomeRamen.Api.Common/Endpoint/Decorators/ValidationCommandHandlerDecorator.cs`

- Wraps an `ICommandHandler<TCommand, TResponse>` (and a separate one for `ICommandHandler<TCommand>`)
- Resolves `IValidator<TCommand>` from DI
- Validates the command **before** delegating to the inner handler
- Throws `CustomValidationException.ValidationFailed(...)` on failure (matching current `ValidationFilter` behaviour)
- This decorator runs after the endpoint has fully bound all parameters — including route IDs — because it receives an already-constructed command object

```csharp
// ValidationCommandHandlerDecorator.cs
internal sealed class ValidationCommandHandlerDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> inner,
    IValidator<TCommand> validator)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
    {
        ValidationResult result = await validator.ValidateAsync(command, cancellationToken);
        if (!result.IsValid)
        {
            throw CustomValidationException.ValidationFailed("Validation failed", result.Errors);
        }
        return await inner.Handle(command, cancellationToken);
    }
}
```

Create the same pattern for the void variant `ICommandHandler<TCommand>` and for `IQueryHandler<TQuery, TResponse>`:

```
ValidationCommandHandlerDecorator.cs       ← for ICommandHandler<TCommand, TResponse>
ValidationVoidCommandHandlerDecorator.cs   ← for ICommandHandler<TCommand>
ValidationQueryHandlerDecorator.cs         ← for IQueryHandler<TQuery, TResponse>
```

#### 1.4 — Create `AuthorizationCommandHandlerDecorator<TCommand, TResponse>` in `MyHomeRamen.Api.Common`

Location: `MyHomeRamen.Api.Common/Endpoint/Decorators/AuthorizationCommandHandlerDecorator.cs`

- Wraps an `ICommandHandler<TCommand, TResponse>` 
- Resolves `IAuthorizationPolicy<TCommand>` from DI
- Checks authorization **after** validation (decorators stack: auth wraps validation wraps core handler)
- Throws `UnauthorizedAccessException` on failure (matching current `AuthorizationFilter` behaviour)

```
AuthorizationCommandHandlerDecorator.cs       ← for ICommandHandler<TCommand, TResponse>
AuthorizationVoidCommandHandlerDecorator.cs   ← for ICommandHandler<TCommand>
AuthorizationQueryHandlerDecorator.cs         ← for IQueryHandler<TQuery, TResponse>
```

#### 1.5 — Add DI registration helpers in `MyHomeRamen.Api.Common/DependencyInjection.cs`

Add extension methods to register handlers with decorator chains automatically based on what policies are registered:

```csharp
// registers: core handler + optional validation decorator + optional authorization decorator
services.AddCommandHandler<TCommand, TResponse, THandler>();
services.AddValidatedCommandHandler<TCommand, TResponse, THandler>();
services.AddAuthorizedValidatedCommandHandler<TCommand, TResponse, THandler>();

services.AddQueryHandler<TQuery, TResponse, THandler>();
services.AddValidatedQueryHandler<TQuery, TResponse, THandler>();
```

The decorator chain (outermost to innermost):
```
AuthorizationDecorator → ValidationDecorator → CoreHandler
```

#### 1.6 — Remove `ValidationFilter` and `AuthorizationFilter` endpoint filter usage from `EndpointBuilderExtensions`

- Keep `MapStandardPost`, `MapStandardGet`, `MapStandardPut`, etc. route registration methods but **remove** `WithValidationFilter<TRequest>()` and `WithAuthenticationFilter<TRequest>()` calls from them
- Remove `MapStandardValidatedPost`, `MapStandardValidatedGet`, `MapStandardAuthenticatedPost`, `MapStandardAuthenticatedGet` overloads that were exclusively used for filter registration (or repurpose them to no-ops to maintain backward compatibility during migration)
- Keep `BaseFilter.cs`, `ValidationFilter.cs`, `AuthorizationFilter.cs` files in place until all endpoints have been migrated; mark them `[Obsolete]`

---

### Step 2: Move Request and Response objects to `MyHomeRamen.Common.Contracts`

**Goal**: `Request` and `Response` records used by both the API and Blazor frontend live in `MyHomeRamen.Common.Contracts` so architecture tests no longer need shape-sync checks.

> **Scope for this plan**: only `CreateCategory` and `GetCategoriesByType`. The same pattern applies to all other features.

#### 2.1 — Create module folder structure in `MyHomeRamen.Common.Contracts`

```
MyHomeRamen.Common.Contracts/
└── Menu/
    └── Categories/
        ├── CreateCategoryRequest.cs
        ├── CreateCategoryResponse.cs
        ├── GetCategoriesByTypeRequest.cs
        └── GetCategoriesByTypeResponse.cs
```

Rules:
- Records must be `public sealed record`
- They must **not** implement `IRequest<T>`, `ICommand`, or `IQuery` — those remain API-layer concerns
- No `RouteParamAttribute` — properties extracted from routes become command-level concerns (see Step 3)
- Use only primitive types or other contracts types — no domain types

#### 2.2 — Update `MyHomeRamen.Common.Contracts` project reference

Ensure `MyHomeRamen.Api` and `MyHomeRamen.Blazor` / `MyHomeRamen.Blazor.Client` already reference `MyHomeRamen.Common.Contracts` (verify — no new project reference changes expected based on existing architecture).

---

### Step 3: Command Implementation — `CreateCategory`

#### 3.1 — Create `CreateCategoryCommand` in `MyHomeRamen.Api`

Location: `MyHomeRamen.Api/Menu/Features/Categories/CreateCategory/CreateCategoryCommand.cs`

```csharp
public sealed record CreateCategoryCommand(
    string Name,
    int CategoryType) : ICommand<Guid>;
```

- This is the **API-internal** object. It receives data already fully bound (body + any route segments).
- Implements `ICommand<Guid>` (not `IRequest<Guid>`).
- No `RouteParamAttribute` needed — command is constructed by the endpoint after model binding.

#### 3.2 — Update `Models/Mappings.cs` for `CreateCategory`

Add a mapping from `CreateCategoryRequest` → `CreateCategoryCommand`:

```csharp
public static CreateCategoryCommand ToCommand(this CreateCategoryRequest request)
    => new(request.Name, request.CategoryType);
```

Keep the existing `ToDomain(this CreateCategoryCommand command, int nextSortOrder)` mapping (updated to accept command instead of request).

#### 3.3 — Update `CreateCategoryValidator` → `CreateCategoryCommandValidator`

Location: `MyHomeRamen.Api/Menu/Features/Categories/CreateCategory/Policies/CreateCategoryCommandValidator.cs`

- Rename class from `CreateCategoryValidator` to `CreateCategoryCommandValidator`
- Change `AbstractValidator<CreateCategoryRequest>` → `AbstractValidator<CreateCategoryCommand>`
- Logic remains identical — validation rules don't change

#### 3.4 — Update `CreateCategoryHandler`

- Change `IRequestHandler<CreateCategoryRequest, Guid>` → `ICommandHandler<CreateCategoryCommand, Guid>`
- Handler receives `CreateCategoryCommand` instead of `CreateCategoryRequest`
- Update `Handle` method parameter type
- Update mapping call to `command.ToDomain(nextSortOrder)`
- Handler no longer calls validator — validation is handled by the decorator

#### 3.5 — Update `CreateCategoryEndpoint`

- Handler parameter changes from `[FromBody] CreateCategoryRequest request` → same `[FromBody] CreateCategoryRequest request` (request from Common.Contracts, bound by framework)
- Injected service changes from `IRequestHandler<CreateCategoryRequest, Guid>` → `ICommandHandler<CreateCategoryCommand, Guid>`
- Inside `HandleAsync`: map request to command before calling handler:
  ```csharp
  CreateCategoryCommand command = request.ToCommand();
  Guid id = await handler.Handle(command, cancellationToken);
  ```
- Remove `MapStandardValidatedPost` → use `MapStandardPost` (validation is now in the decorator, not the filter)

#### 3.6 — Update `CreateCategory` DI Registration

In `MyHomeRamen.Api/Menu/DependencyInjection.cs`, replace manual `IRequestHandler` registration with:

```csharp
services.AddValidatedCommandHandler<CreateCategoryCommand, Guid, CreateCategoryHandler>();
```

This registers: `CreateCategoryHandler` as core + `ValidationCommandHandlerDecorator` wrapping it.

---

### Step 4: Query Implementation — `GetCategoriesByType`

#### 4.1 — Create `GetCategoriesByTypeQuery` in `MyHomeRamen.Api`

Location: `MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesByType/GetCategoriesByTypeQuery.cs`

```csharp
public sealed record GetCategoriesByTypeQuery(int CategoryType) : IQuery<IEnumerable<GetCategoriesByTypeResponse>>;
```

- Implements `IQuery<TResponse>` (not `IRequest<TResponse>`).
- Constructed from the `GetCategoriesByTypeRequest` (from Common.Contracts) in the endpoint.

#### 4.2 — Update `Models/Mappings.cs` for `GetCategoriesByType`

Add a mapping from `GetCategoriesByTypeRequest` → `GetCategoriesByTypeQuery`:

```csharp
public static GetCategoriesByTypeQuery ToQuery(this GetCategoriesByTypeRequest request)
    => new(request.CategoryType);
```

#### 4.3 — Update `GetCategoriesByTypeValidator` → `GetCategoriesByTypeQueryValidator`

Location: `MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesByType/Policies/GetCategoriesByTypeQueryValidator.cs`

- Rename class to `GetCategoriesByTypeQueryValidator`
- Change `AbstractValidator<GetCategoriesByTypeRequest>` → `AbstractValidator<GetCategoriesByTypeQuery>`
- Logic remains identical

#### 4.4 — Update `GetCategoriesByTypeHandler`

- Change `IRequestHandler<GetCategoriesByTypeRequest, IEnumerable<GetCategoriesByTypeResponse>>` → `IQueryHandler<GetCategoriesByTypeQuery, IEnumerable<GetCategoriesByTypeResponse>>`
- Handler receives `GetCategoriesByTypeQuery` instead of `GetCategoriesByTypeRequest`
- Update `Handle` method parameter type
- Handler logic is unchanged — query object has identical properties

#### 4.5 — Update `GetCategoriesByTypeEndpoint`

- Bound parameter stays `[AsParameters] GetCategoriesByTypeRequest request` (from Common.Contracts)
- Injected service changes to `IQueryHandler<GetCategoriesByTypeQuery, IEnumerable<GetCategoriesByTypeResponse>>`
- Inside `HandleAsync`: map request to query before calling handler:
  ```csharp
  GetCategoriesByTypeQuery query = request.ToQuery();
  IEnumerable<GetCategoriesByTypeResponse> response = await handler.Handle(query, cancellationToken);
  ```
- Remove `MapStandardValidatedGet` → use `MapStandardGet` (validation now in decorator)

#### 4.6 — Update `GetCategoriesByType` DI Registration

```csharp
services.AddValidatedQueryHandler<GetCategoriesByTypeQuery, IEnumerable<GetCategoriesByTypeResponse>, GetCategoriesByTypeHandler>();
```

---

### Step 5: Architecture Tests

#### 5.1 — Add naming convention tests in `MyHomeRamen.ArchitectureTests`

Location: `MyHomeRamen.ArchitectureTests/ProjectTests/NamingConventionTests.cs`

New rules to enforce:
- Types ending in `Command` must implement `ICommand` or `ICommand<TResponse>`
- Types ending in `Query` must implement `IQuery<TResponse>`
- Types ending in `CommandHandler` or `QueryHandler` must implement `ICommandHandler` or `IQueryHandler` respectively
- `Request` and `Response` types must reside in `MyHomeRamen.Common.Contracts` (not in `MyHomeRamen.Api` or `MyHomeRamen.Blazor`)
- `Command` and `Query` types must reside in `MyHomeRamen.Api` (not in Common.Contracts or Blazor)

#### 5.2 — Update `ApiToBlazorContractSyncTests`

- `BlazorRequest_ShouldMatch_ApiRequestShape` test: now that Request types are shared from `MyHomeRamen.Common.Contracts`, there is no longer a need to compare shapes between Blazor and API assemblies for these types
- Replace the shape-comparison test with a test that asserts that `Request`/`Response` types used by both Blazor and API are sourced from `MyHomeRamen.Common.Contracts`, not duplicated in either project
- Keep shape-comparison test only for any remaining legacy Request types not yet migrated

---

### Step 6: Tests

#### Unit Tests (`MyHomeRamen.UnitTests`)

No domain model changes → no new domain unit tests required.

New unit tests for decorator behaviour in `MyHomeRamen.UnitTests/Common/Decorators/`:
- `ValidationCommandHandlerDecoratorTests`
  - `Handle_WhenValidationFails_ShouldThrowCustomValidationException`
  - `Handle_WhenValidationPasses_ShouldDelegateToInnerHandler`
- `ValidationQueryHandlerDecoratorTests`
  - `Handle_WhenValidationFails_ShouldThrowCustomValidationException`
  - `Handle_WhenValidationPasses_ShouldDelegateToInnerHandler`
- `AuthorizationCommandHandlerDecoratorTests`
  - `Handle_WhenNotAuthorized_ShouldThrowUnauthorizedAccessException`
  - `Handle_WhenAuthorized_ShouldDelegateToInnerHandler`

#### Integration Tests (`MyHomeRamen.IntegrationTests`)

Existing integration tests for `CreateCategory` and `GetCategoriesByType` should continue to pass without change since the HTTP contract (route, body, response shape) is unchanged. Verify:

- `CreateCategoryTests` — confirm 201 Created, validation 400 responses still work
- `GetCategoriesByTypeTests` — confirm 200 OK, validation 400 responses still work

No new integration test files needed for this refactor; run existing tests to confirm no regression.

#### Architecture Tests (`MyHomeRamen.ArchitectureTests`)

New tests as described in Step 5.

---

## Migration notes

- The refactor should be applied endpoint-by-endpoint. Existing endpoints using `IRequestHandler` + filters continue to work during migration.
- `ValidationFilter` and `AuthorizationFilter` are marked `[Obsolete]` but not removed until all endpoints are migrated.
- No database migrations required — this is a purely structural/pipeline refactor with no domain model changes.
