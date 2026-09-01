---
description: 'Instructions for backend projects'
applyTo: '**/MyHomeRamen.Domain/**/*.cs,**/MyHomeRamen.Api/**/*.cs,**/MyHomeRamen.Infrastructure/**/*.cs,**/MyHomeRamen.Persistance/**/*.cs'
---

# Backend Layer Instructions

Instructions are applicable for all modules.

# 1) Domain Layer (`MyHomeRamen.Domain`)

### 1.1) Aggregate roots & entities
- Inherit from `AuditableEntity`, implement `IEntity<TId>`
- No public constructors — use static methods (e.g., `Product.Create(...)`)
- Have private parameterless constructor for EF Core
- Define a private constructor with `ID` and navigation properties for relation loading
- Properties with private setter

### 1.2) Validators
- `internal static class {Entity}Validator` per Aggregate in the same folder
- Exposes method: `internal static void Validate{Entity}({Entity} entity)`
- Aggregate Factory method **must** call the validator before returning.

### 1.3) Strongly-typed IDs
- `readonly record struct {Entity}Id` with implicit casts to/from `Guid`

## 2) Persistance Layer New

### 2.1) DbContext and unit of work
- `I{Module}DbContext`: e.g. `MyHomeRamen.Features/{Module}/Features/Abstractions/IModuleDbContext.cs`
- implements `IUnitOfWork` from `MyHomeRamen.Features/Common/Repository/IUnitOfWork.cs`, exposes `I{Aggregate}Repository` for each aggregate
- implementation in `MyHomeRamen.Persistance/{Module}/{Module}DbContext` and implements the feature-layer context interface.

### 2.2) Repository, query and specification pattern
- Base repository contract: `IRepository<TEntity, TId>` in `MyHomeRamen.Features/Common/Repository/IRepository.cs`
- Specific aggregate repository interface:
	- extends `IRepository<TEntity, TId>`, exposes `Query()` and `Specification()` methods e.g. `ICategoryRepository`.
- Read-only access via `I{Aggregate}Query` 
- Write operations via `I{Aggregate}Specification` for query entity, `I{Aggregate}Repository` for add/update/remove and `I{Module}DbContext` for SaveChanges
- Implement interfaces directly on DbContext as partial classes:
	- `MyHomeRamen.Persistance\{Module}\{Module}DbContext.cs` implements `IUnitOfWork`
	- `MyHomeRamen.Persistance\{Module}\{Aggregate}\{Aggregate}Query.cs` implements `I{Aggregate}Query`
	- `MyHomeRamen.Persistance\{Module}\{Aggregate}\{Aggregate}Specification.cs` implements `I{Aggregate}Specification`
	- `MyHomeRamen.Persistance\{Module}\{Aggregate}\{Aggregate}Repository.cs` implements `I{Aggregate}Repository`
- Never expose `DbSet<TEtity` or `IQueryable<TEntity>` outside of the DbContext implementation
- Never create repositories etc. for non-aggregate entities / value objects e.g. `PaymentDetails` in ShoppingCart module, use aggregate repository/specification/query instead.

### 2.3) Common patterns
- Entity configurations in `MyHomeRamen.Persistance/{Module}/Configurations`
- Strongly-typed ID converters in `MyHomeRamen.Persistance/{Module}/Converters`
- Migrations in `MyHomeRamen.Persistance/{Module}/Migrations` with `YYYYMMDD_{Description}` naming convention.
- Use `HasConversion` for Enums stored as strings
- Use `AsNoTracking()` for read-only query implementations
- Do not introduce legacy `DbExtensions`-style helpers for new modules; follow the repository/query/specification approach above

### 3) Features layer (`MyHomeRamen.Features`)

### 3.1) REPR + CQRS

**Unified request pipeline (`...MyHomeRamen.Features\Common\Endpoints\`)**:
- requests: `IRequest<TResponse>`, `IRequestHandler<TRequest, TResponse>`
- commands: `ICommand` uses `Unit`; `ICommand<TResponse>` carries response type; queries use `IQuery<TResponse>`
- decorators: generic `ValidationHandler<TRequest, TResponse>` and `AuthorizationHandler<TRequest, TResponse>` implement unified handler interface

DI handled by Scrutor, manual registration not needed.

**Query rules** (GET):
- Inject `I{Module}DbContext` to query handler
- Call `I{Module}DbContext.{Aggregate}.Query().{Method}`
	- non-paginated: expected projected DTO result
	- pagination: expected `PagedResult<TDto>` result that should be mapped to `{Feature}Response` DTO in query handler
- Expected HTTP codes: `200` with `{Feature}Response` body `400`, `403` `500`

Paged query:
- Separate `[AsParameters]` records in endpoint signature
- Required params: `PageParameters`
- Optional params: `OrderParameters`, `{Feature}FilterParameters`
- DB Query signature: `I{Module}DbContext.{Aggregate}.Query().{Method}(filter, order, page, projection, cancellationToken)`
- DB Query returns `PagedResult<TDto>`
- Query response: `{Feature}Response(int Page, int PageSize, int TotalCount, IEnumerable<TDto> Items)` mapped from Query.PageParameters and PagedResult<TDto> in query handler
- Example: `MyHomeRamen.Features\Menu\Features\Products\GetProductsForManage\GetProductsForManageHandler.cs`

**Command rules** (POST / PUT / PATCH / DELETE):
- Inject `I{Module}DbContext` to command handler
- `POST`:
	- Constuct entity from `{Feature}Request`
	- Call `I{Module}DbContext.{Aggregate}.Add(entity)` / `I{Module}DbContext.{Aggregate}.AddRange(entities)`
	- Call `I{Module}DbContext.SaveChangesAsync()`
	- Return `201` with `Location` header or `400`, `403`, `500`

- `PUT` / `PATCH`:
	- Call `I{Module}DbContext.{Aggregate}.Specification().{Method}` to get entity
	- Update entity from `{Feature}Request`
	- Call `I{Module}DbContext.SaveChangesAsync()`
	- Return `200` with ID in `{Feature}Response` body or `400`, `403`, `500`

- `DELETE`:
	- Call `I{Module}DbContext.{Aggregate}.Specification().{Method}` to get entity
	- Call `I{Module}DbContext.{Aggregate}.Remove(entity)` / `I{Module}DbContext.{Aggregate}.RemoveRange(entities)`
	- Call `I{Module}DbContext.SaveChangesAsync()`
	- Return `204` or `400`, `403`, `500`

**Query and Command rules**:
- Validate via `AbstractValidator<T>` in the same folder:
	- primitive types rules live `MyHomeRamen.Common.Contracts.Validators`, always add `WithMessage()`
	- persistence-level rules (e.g. exists, unique, usage) require calling `I{Module}DbContext.{Aggregate}.Exists()` or `I{Module}DbContext.{Aggregate}.Query().{Method}` in validator
	- validator live in the same folder as feature
- Mapping via static `Mappings` class in the same folder


### 3.2) Endpoint configuration
- Implements `IEndpoint`
- Use extension methods from `MyHomeRamen.Api.Common.EndpointBuilderExtensions.cs` for `MapStandardGet<TResponse>`, `MapStandardPost<TResponse>` etc.
- Defines `WithName()`, `WithDescription()`, `WithTags()`
- Allways specify `RequireAuthorization(<PolicyName>)` or add `AllowAnonymous()`
