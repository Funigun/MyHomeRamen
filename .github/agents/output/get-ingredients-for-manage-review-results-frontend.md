- **Date**: 2026-04-06 09:04:33 +02:00
- **Feature**: get-ingredients-for-manage
- **Critical**: 1
- **Warnings**: 2
- **Information**: 0

## Warning

- **Title**: [1] [MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Ingredients/Components/IngredientTable.razor : 42] - UI component directly depends on API response DTO
- **Severity**: Warning
- **Description**: `IngredientTable` accepts `List<GetIngredientsForManageResponse>` directly. Project Blazor rules require decoupling API DTOs from UI models (`{Entity}TableModel`) to keep presentation concerns isolated and maintainable.
- **Solution proposal**: Introduce `IngredientTableModel` in `Components/`, map API responses in page code, and pass `List<IngredientTableModel>` to `IngredientTable`.
- **Implementation status**: ✅ Fixed in iteration 1 — Created `IngredientTableModel.cs` in `Components/` with `FromResponse` factory; updated `IngredientTable.razor` `Items` parameter to `List<IngredientTableModel>`; updated `IngredientsManagementPage.razor` to map via `IngredientTableModel.FromResponse` and removed the direct `Responses` using directive.
