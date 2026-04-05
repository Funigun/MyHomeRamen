# Feature Implementation Plan — GetIngredientById (Backend)

- **Date**: 2025-07-15
- **Feature**: GetIngredientById
- **Module**: Menu
- **Type**: Feature (Query — GET-by-ID endpoint)

---

## 1) Create feature folder and structure

```
MyHomeRamen.Api/
└── Menu/
    └── Features/
        └── Ingredients/
            └── GetIngredientById/
                ├── Models/
                │   ├── GetIngredientByIdRequest.cs
                │   ├── GetIngredientByIdResponse.cs
                │   └── Mappings.cs
                ├── Policies/
                │   └── GetIngredientByIdValidator.cs
                ├── GetIngredientByIdEndpoint.cs
                └── GetIngredientByIdHandler.cs
```

---

## 2) Create primitive rules and contracts

No new primitive validators needed — the only validation is existence check on the ingredient ID.

---

## 3) Create models, DTOs and mappings

### `Models/GetIngredientByIdRequest.cs`
- `public record struct GetIngredientByIdRequest : IRequestId<GetIngredientByIdRequest>, IRequest<GetIngredientByIdResponse>`
- Property: `public Guid Id { get; set; }`
- Implements `IRequestId` for route parameter binding (`{id}`)
- Reference: `DeleteCategoryRequest.cs`

### `Models/GetIngredientByIdResponse.cs`
- `public sealed record GetIngredientByIdResponse(Guid Id, string Name, string Description, decimal Price, IEnumerable<Guid> CategoryIds);`
- Includes `CategoryIds` to pre-fill the edit form category multi-select

### `Models/Mappings.cs`
- `internal static class Mappings` with extension method:
  - `public static GetIngredientByIdResponse ToResponse(this Ingredient ingredient)` → maps `Id.Value`, `Name`, `Description`, `Price`, `Categories.Select(c => (Guid)c.Id)`
- Reference: `GetIngredientsForDropdown/Models/Mappings.cs`

---

## 4) Create IRequestHandler implementation

### `GetIngredientByIdHandler.cs`
- `public sealed class GetIngredientByIdHandler(IMenuDbContext dbContext) : IRequestHandler<GetIngredientByIdRequest, GetIngredientByIdResponse>`
- In `Handle`:
  1. Load ingredient with categories: `dbContext.Ingredients.AsNoTracking().Include(i => i.Categories)` then use expression-based selector for the ID match via `GetBySelectorNotTrackedAsync` — OR — since we need `Include`, query directly:
     ```csharp
     Ingredient ingredient = await dbContext.Ingredients
         .AsNoTracking()
         .Include("_categories")
         .FirstAsync(i => i.Id == (IngredientId)id.Id, cancellationToken);
     ```
  2. Map to response: `ingredient.ToResponse()`
  3. Return `Results.Ok(response)`
- Note: The validator already ensures the ingredient exists, so `FirstAsync` is safe
- Reference: `DeleteCategoryHandler.cs` (entity load pattern)

---

## 5) Create IEndpoint implementation

### `GetIngredientByIdEndpoint.cs`
- `public sealed class GetIngredientByIdEndpoint : IEndpoint`
- `GroupName = "Menu"`
- Maps `MapStandardValidatedGet<GetIngredientByIdRequest, GetIngredientByIdResponse>("ingredients/{id}", HandleAsync)`
- `.WithName("GetIngredientByIdEndpoint")`
- `.WithDescription("Returns the full details of a single ingredient by its ID, including associated category IDs.")`
- `.RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy)`
- Handler method: `HandleAsync(GetIngredientByIdRequest id, [FromServices] IRequestHandler<...> handler, CancellationToken)` → `Results.Ok(response)`
- Parameter name `id` matches route `{id}` for automatic model binding
- Reference: `DeleteCategoryEndpoint.cs`

---

## 6) Create validation policy

### `Policies/GetIngredientByIdValidator.cs`
- `public sealed class GetIngredientByIdValidator : AbstractValidator<GetIngredientByIdRequest>`
- Rules:
  - `RuleFor(x => x.Id).NotEmpty().WithMessage("Ingredient ID must not be empty.")`
  - `.ChildRules` → `MustAsync(IngredientExists(menuDbContext)).WithMessage("Ingredient with the specified ID does not exist.")`
- Private helper: `IngredientExists` calls `menuDbContext.Ingredients.ExistsByIdAsync((IngredientId)id, ct)`
- Reference: `DeleteCategoryValidator.cs` (existence check pattern)

---

## 7) Persistence

No new `DbExtensions` method required — the handler uses `Include` + `FirstAsync` directly since it's a single-entity load with navigation. Existence check reuses the generic `ExistsByIdAsync`.

---

## 8) Unit tests

**Skipped.** The feature contains no domain logic — it's a straightforward single-entity read by primary key.

---

## 9) Integration tests

### File: `MyHomeRamen.IntegrationTests/MenuModule/Ingredients/GetIngredientByIdTests.cs`
- `public sealed class GetIngredientByIdTests(WebApiFactory apiFactory)`

| # | Test method | Description |
|---|---|---|
| 1 | `GetIngredientById_ShouldReturnOk_ForAuthenticatedAdmin` | Seed ingredient, auth as Admin, GET `/api/menu/ingredients/{id}`, assert `200 OK`, correct data including `CategoryIds` |
| 2 | `GetIngredientById_ShouldReturnUnauthorized_ForUnauthenticatedUser` | No auth header, assert `401 Unauthorized` |
| 3 | `GetIngredientById_ShouldReturnForbidden_ForNonAdminRole` | `[Theory] [InlineData(UserRoles.Employee)] [InlineData(UserRoles.Customer)]`, assert `403 Forbidden` |
| 4 | `GetIngredientById_ShouldReturnBadRequest_ForNonExistentId` | Random Guid, auth as Admin, assert `400 Bad Request` |
| 5 | `GetIngredientById_ResponseShouldContainCategoryIds` | Seed ingredient with known categories, assert response `CategoryIds` matches |

- Reference: `GetIngredientsForDropdownTests.cs`, `DeleteCategoryTests.cs`

---

## 10) Architecture tests

**Skipped.** No new module or cross-module dependency introduced.
