# Feature Implementation Plan — DeleteCategory (Backend)

- **Date**: 2025-01-27
- **Feature**: DeleteCategory — delete a single category by ID with conflict and sort-order compaction
- **Module**: Menu
- **Authorization**: RestaurantManagerPolicy (Admin role only)
- **Prerequisite**: `UpdateCategoriesOrder` feature — provides `Category.UpdateSortOrder(int)` method

---

## 1) Create feature folder and structure

```
MyHomeRamen.Api/Menu/Features/Categories/
└── DeleteCategory/
    ├── Models/
    │   └── DeleteCategoryRequest.cs
    ├── Policies/
    │   └── DeleteCategoryValidator.cs
    ├── DeleteCategoryEndpoint.cs
    └── DeleteCategoryHandler.cs
```

---

## 2) Create primitive rules and contracts

No new primitive validators needed.

---

## 3) Domain changes

No new domain changes required — `Category.UpdateSortOrder(int)` is already available from the `UpdateCategoriesOrder` feature and will be reused for re-contiguous sort order assignment after deletion.

---

## 4) Create models, DTOs and mappings

### File: `MyHomeRamen.Api/Menu/Features/Categories/DeleteCategory/Models/DeleteCategoryRequest.cs`
```csharp
public sealed record DeleteCategoryRequest(Guid Id) : IRequest;
```

No `Mappings.cs` — no response DTO (204 No Content), no domain creation from request.

---

## 5) Persistence changes — new DbExtension

### File: `MyHomeRamen.Persistance/Common/DbExtensions.cs`
Add an extension to check if a category is referenced by any child entities. The validator already has the fetched `Category` at that point, so pass the values it knows:

```csharp
public static async Task<bool> IsCategoryReferencedAsync(
    this IMenuDbContext dbContext,
    CategoryId categoryId,
    CategoryType categoryType,
    CancellationToken cancellationToken = default)
```

- If `categoryType == CategoryType.Product` → `dbContext.Products.AnyAsync(p => p.CategoryId == categoryId, ct)`
- If `categoryType == CategoryType.Ingredient` → `dbContext.Ingredients.AnyAsync(i => i.CategoryId == categoryId, ct)`
- Returns `true` if any child reference exists

Also add a query extension for fetching remaining categories after deletion:

```csharp
public static IQueryable<Category> ForCategoryTypeTracked(
    this DbSet<Category> categories,
    CategoryType categoryType)
```
- Same as `ForCategoryType` but **without** `AsNoTracking()` so entities can be updated via change tracking
- Orders by `SortOrder` ascending

> **Note**: Verify that `Product` and `Ingredient` entities expose a `CategoryId` FK property. If the relationship uses a navigation-only pattern, adjust the `AnyAsync` predicate accordingly. Check `Product.cs` and `Ingredient.cs`.

---

## 6) Create IRequestHandler implementation

### File: `MyHomeRamen.Api/Menu/Features/Categories/DeleteCategory/DeleteCategoryHandler.cs`
- Implements `IRequestHandler<DeleteCategoryRequest>`
- Inject `IMenuDbContext`
- The validator guarantees the category exists and has no references before the handler runs, so no guard checks are needed here
- Steps:
  1. Fetch `Category` by `request.Id` via `dbContext.Categories.FindAsync(new CategoryId(request.Id), ct)`
  2. Call `dbContext.Categories.Remove(category)` to stage the delete
  3. Fetch remaining categories of the same `CategoryType` via `dbContext.Categories.ForCategoryTypeTracked(category.CategoryType).ToListAsync(ct)`
  4. Enumerate the list with index and call `category.UpdateSortOrder(index + 1)` on each to assign contiguous 1-based sort order values
  5. Call `dbContext.SaveChangesAsync(ct)` — single round-trip: one `DELETE` + batch `UPDATE` statements via EF Core change tracking

---

## 7) Create IEndpoint implementation

### File: `MyHomeRamen.Api/Menu/Features/Categories/DeleteCategory/DeleteCategoryEndpoint.cs`
- Implements `IEndpoint`
- `GroupName = "Menu"`
- Maps `DELETE "categories/{id:guid}"` using `MapStandardValidatedDelete<DeleteCategoryRequest>` to wire the validation filter
- `WithName("DeleteCategoryEndpoint")`
- `WithDescription("Deletes a category by its ID. Returns 400 if the category does not exist or is still referenced by products or ingredients.")`
- `.RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy)`
- Handler method:
  - Accepts `[AsParameters] DeleteCategoryRequest request`, `[FromServices] IRequestHandler<DeleteCategoryRequest> handler`
  - Calls `handler.Handle(request, ct)`
  - Returns `Results.NoContent()`

> **Note**: `MapStandardDelete` does not have a `Validated` variant yet — check `EndpointBuilderExtensions.cs`. If missing, add `MapStandardValidatedDelete<TRequest>` following the existing `MapStandardValidatedPut<TRequest>` pattern (chain `.WithValidationFilter<TRequest>()`).

---

## 8) Create validation policy

### File: `MyHomeRamen.Api/Menu/Features/Categories/DeleteCategory/Policies/DeleteCategoryValidator.cs`
- Extends `AbstractValidator<DeleteCategoryRequest>`
- Inject `IMenuDbContext`
- Declare a private nullable `_category` field (`Category?`) to avoid a second DB fetch in the reference check
- Rules:
  1. `RuleFor(x => x.Id)` — `.NotEmpty()` with message `"Category ID must not be empty."`
  2. `RuleFor(x => x.Id)` — `.MustAsync(CategoryExistsAsync)` with message `"Category with the specified ID does not exist."` — sets `_category` as a side effect
     - `.DependentRules(() => { ... })` — the following rule only runs when the category was found:
  3. (inside `DependentRules`) `RuleFor(x => x.Id)` — `.MustAsync(CategoryNotReferencedAsync)` with message `"Category is still in use and cannot be deleted."`

```csharp
private async Task<bool> CategoryExistsAsync(Guid id, CancellationToken ct)
{
    _category = await _dbContext.Categories.FindAsync([new CategoryId(id)], ct);
    return _category is not null;
}

private async Task<bool> CategoryNotReferencedAsync(Guid id, CancellationToken ct)
{
    return !await _dbContext.IsCategoryReferencedAsync(_category!.Id, _category!.CategoryType, ct);
}
```

> **HTTP status codes**: both checks surface as `400 Bad Request` via `CustomValidationException` (the path all `MustAsync` failures follow through `ValidationFilter` → `ExceptionMiddleware`). `404` and `409` are not returned for this endpoint.

---

## 7) Create unit tests

### File: `MyHomeRamen.UnitTests/MenuModule/Categories/CategoryValidationTests.cs`
No additional unit tests needed for DeleteCategory specifically — `UpdateSortOrder` tests were already added with UpdateCategoriesOrder feature. The delete logic is handler-level (persistence + orchestration), not domain validation.

---

## 8) Create integration tests

### File: `MyHomeRamen.IntegrationTests/MenuModule/Categories/DeleteCategoryTests.cs`

Test cases:
- `DeleteCategory_ShouldReturnNoContent_ForValidRequest` — seed categories, delete one, assert 204
- `DeleteCategory_ShouldReturnBadRequest_ForNonExistentCategory` — send random `Guid.NewGuid()`, assert 400 with validation error message containing `"does not exist"`
- `DeleteCategory_ShouldReturnBadRequest_ForCategoryWithProducts` — seed a category with associated products, attempt delete, assert 400 with validation error message containing `"still in use"`
- `DeleteCategory_ShouldReturnBadRequest_ForCategoryWithIngredients` — seed a category with associated ingredients, attempt delete, assert 400 with validation error message containing `"still in use"`
- `DeleteCategory_ShouldReturnBadRequest_ForEmptyId` — send `Guid.Empty`, assert 400
- `DeleteCategory_ShouldReturnUnauthorized_ForAnonymousUser` — no auth header, assert 401
- `DeleteCategory_ShouldReturnForbidden_ForNonAdminUser` — `[InlineData(UserRoles.Employee)]`, `[InlineData(UserRoles.Customer)]`, assert 403
- `DeleteCategory_ShouldReorderRemainingCategories_AfterDeletion` — seed 3 categories (sortOrder 1,2,3), delete the middle one, GET remaining by type, assert sort orders are `[1, 2]` (re-contiguous)

---

## 9) Create architecture tests

No new architecture tests needed — existing module boundary tests already cover the Menu module.

---

## 10) Create system tests

Skip — no distributed workflow involved, single API call with DB persistence only.
