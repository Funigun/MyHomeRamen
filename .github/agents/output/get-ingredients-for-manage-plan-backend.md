# Feature Implementation Plan — GetIngredientsForManage (Backend)

- **Date**: 2025-07-15
- **Feature**: GetIngredientsForManage
- **Module**: Menu
- **Type**: Feature (Query — GET endpoint)

---

## 1) Create feature folder and structure

```
MyHomeRamen.Api/
└── Menu/
    └── Features/
        └── Ingredients/
            └── GetIngredientsForManage/
                ├── Models/
                │   ├── GetIngredientsForManageRequest.cs
                │   ├── GetIngredientsForManageResponse.cs
                │   └── Mappings.cs
                ├── Policies/
                │   └── GetIngredientsForManageValidator.cs
                ├── GetIngredientsForManageEndpoint.cs
                └── GetIngredientsForManageHandler.cs
```

---

## 2) Create primitive rules and contracts

No new primitive validators needed — the only validation is an optional `MaximumLength` guard on `Name`, which can reference the existing `IngredientConstants.MaxNameLength` directly in the feature validator.

---

## 3) Create models, DTOs and mappings

### `Models/GetIngredientsForManageRequest.cs`
- `public sealed record GetIngredientsForManageRequest(string? Name, IEnumerable<Guid>? CategoryIds) : IRequest<IEnumerable<GetIngredientsForManageResponse>>;`
- Bound via `[AsParameters]` in the endpoint (query string binding)

### `Models/GetIngredientsForManageResponse.cs`
- `public sealed record GetIngredientsForManageResponse(Guid Id, string Name, string Description);`
- Categories intentionally excluded

### `Models/Mappings.cs`
- `internal static class Mappings` with extension method:
  - `public static GetIngredientsForManageResponse ToResponse(this Ingredient ingredient)` → maps `Id.Value`, `Name`, `Description`
- Reference: `GetIngredientsForDropdown/Models/Mappings.cs`

---

## 4) Create IRequestHandler implementation

### `GetIngredientsForManageHandler.cs`
- `public sealed class GetIngredientsForManageHandler(IMenuDbContext dbContext) : IRequestHandler<GetIngredientsForManageRequest, IEnumerable<GetIngredientsForManageResponse>>`
- In `Handle`:
  1. Call `dbContext.Ingredients.ForManage(request.Name, request.CategoryIds)`
  2. Project `.Select(i => i.ToResponse())`
  3. Materialize with `.ToListAsync(cancellationToken)`
  4. Return result
- Reference: `GetIngredientsForDropdownHandler.cs`

---

## 5) Create IEndpoint implementation

### `GetIngredientsForManageEndpoint.cs`
- `public sealed class GetIngredientsForManageEndpoint : IEndpoint`
- `GroupName = "Menu"`
- Maps `MapStandardValidatedGet<GetIngredientsForManageRequest, IEnumerable<GetIngredientsForManageResponse>>("ingredients/manage", HandleAsync)`
- `.WithName("GetIngredientsForManageEndpoint")`
- `.WithDescription("Returns a filtered list of ingredients for the admin management view. Supports optional name (contains, case-insensitive) and category ID filters.")`
- `.RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy)`
- Handler method: `HandleAsync([AsParameters] GetIngredientsForManageRequest request, [FromServices] IRequestHandler<...> handler, CancellationToken)` → `Results.Ok(response)`
- Reference: `GetCategoriesByTypeEndpoint.cs`

---

## 6) Create validation policy

### `Policies/GetIngredientsForManageValidator.cs`
- `public sealed class GetIngredientsForManageValidator : AbstractValidator<GetIngredientsForManageRequest>`
- Rules:
  - `RuleFor(x => x.Name).MaximumLength(IngredientConstants.MaxNameLength)` — guard when provided (FluentValidation skips null by default for `MaximumLength`)
- Reference: `GetCategoriesByTypeValidator.cs`

---

## 7) Create persistence extension

### `MyHomeRamen.Persistance/Common/DbExtensions.cs`
- Add `ForManage` extension method:

```csharp
public static IQueryable<Ingredient> ForManage(
    this DbSet<Ingredient> ingredients,
    string? name,
    IEnumerable<Guid>? categoryIds)
{
    IQueryable<Ingredient> query = ingredients.AsNoTracking();

    if (!string.IsNullOrWhiteSpace(name))
    {
        query = query.Where(i => i.Name.ToLower().Contains(name.ToLower()));
    }

    if (categoryIds is not null && categoryIds.Any())
    {
        List<CategoryId> ids = categoryIds.Select(id => (CategoryId)id).ToList();
        query = query.Where(i => i.Categories.Any(c => ids.Contains(c.Id)));
    }

    return query.OrderBy(i => i.Name);
}
```

- Semantic: **union** (any) — returns ingredients belonging to **any** of the specified categories
- Reference: existing `ForDropdown` extension on `DbSet<Ingredient>`

---

## 8) Unit tests

**Skipped.** The feature contains no domain logic — the handler is a filtered read that delegates to EF Core.

---

## 9) Integration tests

### File: `MyHomeRamen.IntegrationTests/MenuModule/Ingredients/GetIngredientsForManageTests.cs`
- `public sealed class GetIngredientsForManageTests(WebApiFactory apiFactory)`

| # | Test method | Description |
|---|---|---|
| 1 | `GetIngredientsForManage_ShouldReturnOk_ForAuthenticatedAdmin` | Auth as Admin, GET `/api/menu/ingredients/manage`, assert `200 OK`, non-null non-empty list |
| 2 | `GetIngredientsForManage_ShouldReturnUnauthorized_ForUnauthenticatedUser` | No auth header, assert `401 Unauthorized` |
| 3 | `GetIngredientsForManage_ShouldReturnForbidden_ForNonAdminRole` | `[Theory] [InlineData(UserRoles.Employee)] [InlineData(UserRoles.Customer)]`, assert `403 Forbidden` |
| 4 | `GetIngredientsForManage_ShouldFilterByName_WhenNameProvided` | Seed ingredient with known name, query with partial name filter, assert only matching ingredients returned |
| 5 | `GetIngredientsForManage_ShouldFilterByCategories_WhenCategoryIdsProvided` | Use seeded ingredient category ID, query with `categoryIds=`, assert only ingredients belonging to that category are returned |
| 6 | `GetIngredientsForManage_ShouldReturnEmptyList_WhenNoIngredientsMatchFilters` | Query with a name that doesn't match any ingredient, assert `200 OK` with empty list |
| 7 | `GetIngredientsForManage_ResponseShouldNotContainCategories` | Deserialize response, assert each item has only `Id`, `Name`, `Description` properties (no `Categories` or `CategoryIds`) |

- Reference: `GetIngredientsForDropdownTests.cs`, `DeleteCategoryTests.cs`

---

## 10) Architecture tests

**Skipped.** No new module or cross-module dependency introduced.
