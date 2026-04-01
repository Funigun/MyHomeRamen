- **Date**: 2025-07-14
- **Feature**: GetCategoriesForManage + GetIngredientsForDropdown (branch: feature/get_categories_for_manage)
- **Critical**: 0
- **Warnings**: 1
- **Information**: 1

---

## [1] [DbExtensions.cs : 88-97] - ForManage duplicates ForDropdown implementation

- **Severity**: Warning
- **Description**: `ForManage` and `ForDropdown` for `Category` are currently identical in implementation. Both execute `categories.AsNoTracking().Where(c => c.CategoryType == categoryType).OrderBy(c => c.SortOrder)`. Duplicating query logic across two extension methods introduces a maintenance risk: a future change to one (e.g., an additional `.Include(...)`, a changed ordering) will silently not apply to the other.
- **Solution proposal**: Have `ForManage` delegate to `ForDropdown` since both currently produce the same query shape. When the manage query needs to diverge (e.g., selecting additional fields or applying different filters), update it independently at that point:
  ```csharp
  public static IQueryable<Domain.Menu.Categories.Category> ForManage(
      this DbSet<Domain.Menu.Categories.Category> categories,
      Domain.Menu.Categories.CategoryType categoryType)
  {
      return categories.ForDropdown(categoryType);
  }
  ```

---

## [2] [GetCategoriesForManageHandler.cs : 18-28] - Two sequential DB round-trips

- **Severity**: Information
- **Description**: The handler executes two sequential `ToListAsync` calls — one for `CategoryType.Product` and one for `CategoryType.Ingredient`. These are independent queries with no data dependency between them, so they unnecessarily block each other. For small category lists this has negligible impact, but it is worth flagging for future awareness.
- **Solution proposal**: EF Core does not support concurrent async operations on the same `DbContext` instance, so `Task.WhenAll` cannot be applied directly here. Keep the sequential approach as-is. A future optimization path would be a single query returning all categories for the restaurant, then grouping by `CategoryType` in memory — this trades the double round-trip for a slightly larger single result set.
