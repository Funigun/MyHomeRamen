# Patterns

This file contains canonical patterns that agents must follow when generating code.
It replaces runtime codebase scanning — agents load this file instead of searching for existing patterns.

---

## Domain Patterns

| Concept | File path | Description |
|---|---|---|
| Entity ID | `MyHomeRamen.Domain\Menu\Products\ProductId.cs` | Strongly-typed ID |
| Entity class | `MyHomeRamen.Domain\Menu\Ingredients\Ingredient.cs` | Example entity class with parameterless constructor, factory methods, domain logic methods and validator |

---

## Persistence Patterns

| Concept | File path | Description |
|---|---|---|
| EF Core configuration | `MyHomeRamen.Persistance\Menu\Configurations\ProductConfiguration.cs` | `IEntityTypeConfiguration<T>` implementation; configures keys, required properties, precision, and relationships |
| DbContext extensions | `MyHomeRamen.Persistance\Menu\Extensions\ProductDbExtensions.cs` | `partial static DbExtensions` class using C# 14 extension members syntax scoped to `IQueryable<T>` |
| Generic repository helpers | `MyHomeRamen.Persistance\Common\RepositoryDbExtensions.cs` | Shared helpers: `Paged`, `Exists`, `GetList`, `GetById`, `GetByIds` |

### DbContext extension rules
- Namespace: `MyHomeRamen.Persistance.Common`
- Class: `public static partial class DbExtensions`
- Use C# 14 `extension(IQueryable<T> set) { ... }` syntax
- Each file is scoped to one entity type

---

## API Layer Patterns

### Endpoint patterns

| Concept | File path | Description |
|---|---|---|
| POST endpoint | `MyHomeRamen.Api\Menu\Features\Products\Create\CreateProductEndpoint.cs` | `IEndpoint`; uses `MapStandardPost`; `[FromBody]` request; returns `Results.Created` with Location header |
| PUT endpoint | `MyHomeRamen.Api\Menu\Features\Products\Update\UpdateProductEndpoint.cs` | `IEndpoint`; uses `MapStandardPut`; `[FromRoute] Guid id` + `[FromBody]` request; returns `Results.Ok` |
| DELETE endpoint | `MyHomeRamen.Api\Menu\Features\Ingredients\Delete\DeleteIngredientEndpoint.cs` | `IEndpoint`; uses `MapStandardDelete`; `[FromRoute] Guid id`; returns `TypedResults.NoContent()` |
| GET single | `MyHomeRamen.Api\Menu\Features\Ingredients\GetById\GetIngredientByIdEndpoint.cs` | `IEndpoint`; uses `MapStandardGet`; `[FromRoute] Guid id`; returns `Results.Ok` |
| GET list + pagination | `MyHomeRamen.Api\Menu\Features\Ingredients\GetForManage\GetIngredientsForManageEndpoint.cs` | `IEndpoint`; uses `[AsParameters]` for query string binding; accepts `PageParameters`; returns `Results.Ok` |

### Handler patterns

| Concept | File path | Description |
|---|---|---|
| Command handler | `MyHomeRamen.Api\Menu\Features\Products\Create\CreateProductHandler.cs` | Sealed class; primary constructor DI; implements `ICommandHandler<TCommand, TResult>`; calls `SaveChangesAsync` once after all changes |
| Query handler | `MyHomeRamen.Api\Menu\Features\Ingredients\GetById\GetIngredientByIdHandler.cs` | Sealed class; primary constructor DI; implements `IQueryHandler<TQuery, TResponse>`; uses `AsNoTracking` queries |

### Validator patterns

| Concept | File path | Description |
|---|---|---|
| Request validator | `MyHomeRamen.Api\Menu\Features\Products\UpdateProduct\UpdateProductValidator.cs` | Inherits `AbstractValidator<TCommand>`; route ID accessed directly via the command property (e.g. `command.Id`); async DB existence checks via `IMenuDbContext` DB extensions; validators are auto-discovered — never register manually |

---

## Test Patterns

| Concept | File path | Description |
|---|---|---|
| Unit test — valid creation | `MyHomeRamen.UnitTests\MenuModule\Products\ProductValidationTests.cs` | Arrange/Act/Assert; assert property values on the returned entity |
| Unit test — domain exception | `MyHomeRamen.UnitTests\MenuModule\Products\ProductValidationTests.cs` | `Assert.Throws<DomainException>`; assert `exception.Message` matches `{Entity}Errors.{Rule}().Message` |
| Integration test — happy path | `MyHomeRamen.IntegrationTests\MenuModule\Products\CreateProductTests.cs` | Uses `WebApiFactory`; builds `HttpRequestMessage` with `HttpClientExtensions`; asserts status code and headers |
| Integration test — authorization | `MyHomeRamen.IntegrationTests\MenuModule\Products\CreateProductTests.cs` | `[Theory][InlineData(UserRoles.X)]`; asserts `Forbidden` for non-authorized roles |

---

## Common Mistakes

| Mistake | Correct approach |
|---|---|
| Using `var` | Always declare the explicit type — `var` is a build error in this project |
| Injecting via constructor field | Use primary constructors: `public sealed class Foo(IBar bar)` |
| Calling `SaveChangesAsync` in loops | Batch all changes, then call once |
| Using `FirstOrDefaultAsync` without null check | Use `FirstAsync` when existence is already validated |
| Registering validator manually | Validators are auto-discovered — do not add manual `services.AddValidator<>()` calls |
| Namespace mismatch | Namespace must exactly mirror the folder path under `MyHomeRamen.*` |