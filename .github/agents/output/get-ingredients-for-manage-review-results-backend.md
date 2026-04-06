- **Date**: 2026-04-06 09:04:33 +02:00
- **Feature**: get-ingredients-for-manage
- **Critical**: 0
- **Warnings**: 2
- **Information**: 0

## Warning

- **Title**: [1] [MyHomeRamen.IntegrationTests/MenuModule/Ingredients/GetIngredientsForManageTests.cs : 86] - Category filter test does not verify filter correctness
- **Severity**: Warning
- **Description**: `GetIngredientsForManage_ShouldFilterByCategories_WhenCategoryIdsProvided` only checks for `200 OK`, non-null, and non-empty response. It does not assert that returned ingredients actually belong to the requested category. This can allow regressions where filtering is broken but still returns any seeded data.
- **Solution proposal**: Assert that every returned item is linked to the requested category (e.g., by comparing returned IDs with seeded `DataGenerator.GeneratedIngredients` that contain the category), or enrich response/test data to validate category membership explicitly.
- **Implementation status**: ✅ Fixed in iteration 1 — Added `expectedIngredientIds` computed from `DataGenerator.GeneratedIngredients` filtered by the requested `categoryId`, then asserted `Assert.All(result, i => Assert.Contains(i.Id, expectedIngredientIds))` in `GetIngredientsForManageTests.cs`.

- **Title**: [2] [MyHomeRamen.IntegrationTests/MenuModule/Ingredients/GetIngredientsForManageTests.cs : 127] - Test name and assertions are not aligned
- **Severity**: Warning
- **Description**: `GetIngredientsForManage_ResponseShouldNotContainCategories` suggests verification that category data is excluded from response shape, but assertions only validate `Id`, `Name`, and `Description` are populated. This is not an intent/implementation match per test review rules.
- **Solution proposal**: Remove test 
- **Implementation status**: ✅ Fixed in iteration 1 — Removed `GetIngredientsForManage_ResponseShouldNotContainCategories` from `GetIngredientsForManageTests.cs`.
