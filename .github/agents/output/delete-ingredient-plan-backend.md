# Feature Implementation Plan — DeleteIngredient (Backend)

- **Date**: 2025-07-15
- **Feature**: DeleteIngredient
- **Module**: Menu
- **Type**: Feature (Command — DELETE endpoint)

---

## 1) Create feature folder and structure

```
MyHomeRamen.Api/
└── Menu/
    └── Features/
        └── Ingredients/
            └── DeleteIngredient/
                ├── Models/
                │   └── DeleteIngredientRequest.cs
                ├── Policies/
                │   └── DeleteIngredientValidator.cs
                ├── DeleteIngredientEndpoint.cs
                └── DeleteIngredientHandler.cs
```

No response model needed — `DELETE` returns `204 No Content`.

---

## 2) Create primitive rules and contracts

No new primitive validators needed — validation is ID existence and usage-guard checks.

---

## 3) Create models, DTOs and mappings

### `Models/DeleteIngredientRequest.cs`
- `public record struct DeleteIngredientRequest : IRequestId<DeleteIngredientRequest>, IRequest<IResult>`
- Property: `public Guid Id { get; set; }`
- Implements `IRequestId` for route parameter binding (`{id}`)
- Reference: `DeleteCategoryRequest.cs`

No mappings or response model — `DELETE` returns `204 No Content`.

---

## 4) Create IRequestHandler implementation

### `DeleteIngredientHandler.cs`
- `public sealed class DeleteIngredientHandler(IMenuDbContext dbContext) : IRequestHandler<DeleteIngredientRequest, IResult>`
- In `Handle`:
  1. Load ingredient: `dbContext.Ingredients.GetBySelectorAsync((IngredientId)id.Id, cancellationToken)`
  2. Remove: `dbContext.Ingredients.Remove(ingredient)`
  3. Save: `await dbContext.SaveChangesAsync(cancellationToken)`
  4. Return `Results.NoContent()`
- Note: No resequencing needed (ingredients have no sort order unlike categories)
- Reference: `DeleteCategoryHandler.cs`

---

## 5) Create IEndpoint implementation

### `DeleteIngredientEndpoint.cs`
- `public sealed class DeleteIngredientEndpoint : IEndpoint`
- `GroupName = "Menu"`
- Maps `MapStandardValidatedDelete<DeleteIngredientRequest>("ingredients/{id}", HandleAsync)`
- `.WithName("DeleteIngredientEndpoint")`
- `.WithDescription("Deletes an ingredient by its ID. Validates that the ingredient exists and is not used by any product.")`
- `.RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy)`
- Handler method: `HandleAsync(DeleteIngredientRequest id, [FromServices] IRequestHandler<...> handler, CancellationToken)` → returns handler result
- Parameter name `id` matches route `{id}` for automatic model binding
- Reference: `DeleteCategoryEndpoint.cs`

---

## 6) Create validation policy

### `Policies/DeleteIngredientValidator.cs`
- `public sealed class DeleteIngredientValidator : AbstractValidator<DeleteIngredientRequest>`
- Constructor injects `IMenuDbContext menuDbContext`
- Rules:
  ```
  RuleFor(x => x.Id)
      .NotEmpty().WithMessage("Ingredient ID must not be empty.")
      .ChildRules(id =>
      {
          id.RuleFor(id => id)
              .MustAsync(IngredientExists(menuDbContext))
              .WithMessage("Ingredient with the specified ID does not exist.");

          id.RuleFor(id => id)
              .MustAsync(IngredientIsNotUsedAsBaseIngredient(menuDbContext))
              .WithMessage("Ingredient is used as a base ingredient by one or more products and cannot be deleted.");

          id.RuleFor(id => id)
              .MustAsync(IngredientIsNotUsedAsCustomIngredient(menuDbContext))
              .WithMessage("Ingredient is used as an additional ingredient by one or more products and cannot be deleted.");
      });
  ```
- Private helpers:
  - `IngredientExists` → `menuDbContext.Ingredients.ExistsByIdAsync((IngredientId)id, ct)`
  - `IngredientIsNotUsedAsBaseIngredient` → `!await menuDbContext.Products.IsIngredientUsedAsBaseByProductAsync((IngredientId)id, ct)`
  - `IngredientIsNotUsedAsCustomIngredient` → `!await menuDbContext.Products.IsIngredientUsedAsCustomByProductAsync((IngredientId)id, ct)`
- Reference: `DeleteCategoryValidator.cs` (ChildRules pattern, existence + usage checks)

---

## 7) Create persistence extensions

### `MyHomeRamen.Persistance/Common/DbExtensions.cs`
Add two new extension methods:

#### `IsIngredientUsedAsBaseByProductAsync`
```csharp
public static async Task<bool> IsIngredientUsedAsBaseByProductAsync(
    this IQueryable<Product> query,
    IngredientId ingredientId,
    CancellationToken cancellationToken = default)
{
    return await query.AnyAsync(
        p => p.BaseIngredients.Any(i => i.Id == ingredientId),
        cancellationToken);
}
```

#### `IsIngredientUsedAsCustomByProductAsync`
```csharp
public static async Task<bool> IsIngredientUsedAsCustomByProductAsync(
    this IQueryable<Product> query,
    IngredientId ingredientId,
    CancellationToken cancellationToken = default)
{
    return await query.AnyAsync(
        p => p.CustomIngredients.Any(i => i.Id == ingredientId),
        cancellationToken);
}
```

- Reference: `IsCategoryUsedByProductAsync`, `IsCategoryUsedByIngredientAsync` in `DbExtensions.cs`
- Note: Split into two methods (base vs custom) to provide specific error messages indicating _which_ usage prevents deletion

---

## 8) Unit tests

**Skipped.** No domain logic exercised beyond the removal — no resequencing or domain events.

---

## 9) Integration tests

### File: `MyHomeRamen.IntegrationTests/MenuModule/Ingredients/DeleteIngredientTests.cs`
- `public sealed class DeleteIngredientTests(WebApiFactory apiFactory)`

| # | Test method | Description |
|---|---|---|
| 1 | `DeleteIngredient_ShouldReturnNoContent_ForValidId` | Seed standalone ingredient (not used by any product), auth as Admin, DELETE, assert `204 No Content`, verify ingredient no longer exists in DB |
| 2 | `DeleteIngredient_ShouldReturnUnauthorized_ForUnauthenticatedUser` | No auth header, assert `401 Unauthorized` |
| 3 | `DeleteIngredient_ShouldReturnForbidden_ForNonAdminRole` | `[Theory] [InlineData(UserRoles.Employee)] [InlineData(UserRoles.Customer)]`, assert `403 Forbidden` |
| 4 | `DeleteIngredient_ShouldReturnBadRequest_ForNonExistentId` | Random Guid, auth as Admin, assert `400 Bad Request` |
| 5 | `DeleteIngredient_ShouldReturnBadRequest_WhenIngredientIsUsedAsBaseIngredient` | Use seeded ingredient that is a base ingredient of a product (from `DataGenerator.GeneratedProducts`), assert `400 Bad Request` |
| 6 | `DeleteIngredient_ShouldReturnBadRequest_WhenIngredientIsUsedAsCustomIngredient` | Use seeded ingredient that is a custom ingredient of a product (from `DataGenerator.GeneratedProducts`), assert `400 Bad Request` |

- Reference: `DeleteCategoryTests.cs`, `GetIngredientsForDropdownTests.cs`
- Note: Test 1 requires seeding a new ingredient that is NOT referenced by any product to avoid interference with other tests

---

## 10) Architecture tests

**Skipped.** No new module or cross-module dependency introduced.
