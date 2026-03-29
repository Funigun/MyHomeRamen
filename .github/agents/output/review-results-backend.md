# Backend Review Results

- **Date**: 2025-07-16
- **Feature**: GetCategoriesForDropdown — returns a lightweight list of categories filtered by `CategoryType`, ordered by `SortOrder`, for use in dropdown selectors.
- **Critical**: 0
- **Warnings**: 2
- **Information**: 2

---

## Architecture Tests

✅ Passed — 110 tests, 0 failures.

---

## Issues

---

### [1] [GetCategoriesForDropdownHandler.cs : 16] — Inline query must be extracted to a DB extension method

**Severity**: Warning

**Description**:
The handler builds the entire filtered, ordered query inline:

```csharp
return await dbContext.Categories
    .AsNoTracking()
    .Where(c => c.CategoryType == categoryType)
    .OrderBy(c => c.SortOrder)
    .Select(c => new GetCategoriesForDropdownResponse(c.Id.Value, c.Name))
    .ToListAsync(cancellationToken);
```

The backend instructions (Section 2.2 — DB Extensions, **mandatory**) state:
> "All custom queries, existence checks, and uniqueness checks **must** be extension methods on `IQueryable<T>` or specific `DbSet<T>` types. Place them in `MyHomeRamen.Persistance.Common.DbExtensions`."

An inline query with business predicates (`Where`, `OrderBy`) in a handler is exactly what this rule prohibits. For precedent, see `IsCategoryNameUniqueAsync` in the persistence layer.

**Solution proposal**:
The DB extension cannot return `GetCategoriesForDropdownResponse` — the persistence layer has no reference to the API layer and adding one would create a circular dependency. The DTO must not be moved to a shared project either.

Instead, the extension should own only the reusable query shape (filter + ordering) and return `IQueryable<Category>`. The projection using `Mappings.ToResponse()` stays in the handler, which is the only layer that knows about both `Category` and `GetCategoriesForDropdownResponse`.

Create `CategoryDbExtensions.cs` in `MyHomeRamen.Persistance/Common/DbExtensions/` with:

```csharp
internal static class CategoryDbExtensions
{
    internal static IQueryable<Category> ForDropdown(
        this DbSet<Category> categories,
        CategoryType categoryType)
    {
        return categories
            .AsNoTracking()
            .Where(c => c.CategoryType == categoryType)
            .OrderBy(c => c.SortOrder);
    }
}
```

The handler becomes:

```csharp
return await dbContext.Categories
    .ForDropdown(categoryType)
    .Select(c => c.ToResponse())
    .ToListAsync(cancellationToken);
```

`Mappings.ToResponse()` handles the projection at the handler boundary, which is correct — the persistence extension remains DTO-agnostic and reusable.
- **Implementation status**: ✅ Fixed in iteration 1 — Added `public static IQueryable<Category> ForDropdown(this DbSet<Category> categories, CategoryType categoryType)` to `MyHomeRamen.Persistance/Common/DbExtensions.cs`; handler in `GetCategoriesForDropdownHandler.cs` now calls `.ForDropdown(categoryType)` and removed the inline `AsNoTracking/Where/OrderBy` chain; added `using MyHomeRamen.Persistance.Common` to the handler.

---

### [2] [GetCategoriesForDropdown/Models/Mappings.cs : 7] — ~~`ToResponse()` is dead code~~ ✅ Resolved

**Severity**: ~~Warning~~ Resolved

**Description**:
The original review incorrectly flagged `ToResponse()` as dead code. The handler calls `.Select(c => c.ToResponse())` — EF Core evaluates the `Select` projection client-side after executing the SQL query for the `Where`/`OrderBy` clauses, so the mapping method IS used. The `Mappings.cs` correctly satisfies the project mapping convention.

No action required.

---

### [3] [GetCategoriesForDropdownTests.cs : 40] — `ShouldReturnOkWithList` test is missing `Assert.NotEmpty`

**Severity**: Warning

**Description**:
The test name `GetCategoriesForDropdown_ShouldReturnOkWithList_ForValidCategoryType` implies the response contains a non-empty list. However, the assertions are:

```csharp
Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
Assert.NotNull(result);
```

`Assert.NotNull(result)` only confirms deserialization succeeded (an empty `[]` payload also deserializes to a non-null empty enumerable). The "WithList" part of the test name is not validated.

Since `DataSeeder.SeedMenuModule` seeds product categories, a non-empty result for `CategoryType.Product` is expected and should be asserted.

**Solution proposal**:
Add `Assert.NotEmpty(result)` immediately after `Assert.NotNull(result)`:

```csharp
Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
Assert.NotNull(result);
Assert.NotEmpty(result);
```
- **Implementation status**: ✅ Fixed in iteration 1 — Added `Assert.NotEmpty(result)` after `Assert.NotNull(result)` in `GetCategoriesForDropdown_ShouldReturnOkWithList_ForValidCategoryType` in `GetCategoriesForDropdownTests.cs`.

---

### [4]

**Severity**: Information

**Description**:
The endpoint enforces `RestaurantManagerPolicy`. The test method is named:

```
GetCategoriesForDropdown_ShouldReturnForbidden_ForNonAdminUser
```

The term "NonAdminUser" references `UserRoles.Admin` which internally maps to `RestaurantManagerPolicy`, but this is not obvious from the name alone. The naming convention should reflect the business policy rather than the internal enum name.

**Solution proposal**:
Rename to:
```
GetCategoriesForDropdown_ShouldReturnForbidden_ForNonManagerRole
```
- **Implementation status**: ✅ Fixed in iteration 1 — Renamed method to `GetCategoriesForDropdown_ShouldReturnForbidden_ForNonManagerRole` in `GetCategoriesForDropdownTests.cs`.

---

### [5] [GetCategoriesForDropdownTests.cs : 50] — Ordering test constructs requests manually instead of using `DataGenerator`

**Severity**: Information

**Description**:
`GetCategoriesForDropdown_ShouldReturnCategoriesOrderedBySortOrder` manually constructs `CreateCategoryRequest` objects with raw string names:

```csharp
string firstName = $"OrderTestCat{Guid.NewGuid():N}";
CreateCategoryRequest firstRequest = new(firstName, categoryType);
```

Per the backend-tests instructions (Section 3.2.1), test data should be produced by `DataGenerator` helpers (e.g., `DataGenerator.GenerateValidCategory().ToCreateCategoryRequest()`) so that test cases stay in sync with domain validation constants and boundary values.

**Solution proposal**:
Replace the inline request construction with:

```csharp
CreateCategoryRequest firstRequest  = DataGenerator.GenerateValidCategory().ToCreateCategoryRequest();
CreateCategoryRequest secondRequest = DataGenerator.GenerateValidCategory().ToCreateCategoryRequest();
CreateCategoryRequest thirdRequest  = DataGenerator.GenerateValidCategory().ToCreateCategoryRequest();
```

If `DataGenerator.GenerateValidCategory()` always generates `CategoryType.Product` categories this removes the need for the `categoryType` constant entirely; otherwise pass `CategoryType.Product` as a parameter if the generator supports it.
- **Implementation status**: ✅ Fixed in iteration 1 — Added `GenerateValidCategory(CategoryType categoryType)` overload to `DataGenerator.cs`; replaced inline `string firstName/secondName/thirdName` + `new CreateCategoryRequest(...)` construction with `DataGenerator.GenerateValidCategory(CategoryType.Product).ToCreateCategoryRequest()` in `GetCategoriesForDropdownTests.cs`.

---

```
Drax Reviewer: ✓ Review complete
Drax Reviewer: Critical: 0 | Warnings: 2 | Information: 2
```
