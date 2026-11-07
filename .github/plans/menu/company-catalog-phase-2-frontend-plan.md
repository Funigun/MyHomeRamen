# Frontend Plan: Menu - Phase 2 Company Catalog

## 1. Problem

Company catalog screens must manage ingredients, products, and taxonomy without treating catalog products as priced restaurant offers. Remove catalog product prices, replace product deletion with retirement, and keep categories as taxonomy rather than customer menu sections.

## 2. File tree

```text
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/
├── Categories/
│   ├── CategoryManagementResources.cs [modify]
│   ├── CategoryManagementResources.resx [modify]
│   ├── CategoryManagementView.razor [modify]
│   ├── Components/
│   │   ├── CategoryTable.razor [modify]
│   │   └── CreateCategoryForm.razor [modify]
│   ├── IngredientCategoriesPage.razor [modify]
│   ├── Models/
│   │   ├── CategoryModel.cs [modify]
│   │   ├── CategorySortOrderValidator.cs [modify]
│   │   └── CategoryValidator.cs [modify]
│   ├── ProductCategoriesPage.razor [modify]
│   └── Shared/
│       ├── CategoryApiClient.cs [modify]
│       └── CategoryRoutes.cs [modify]
├── Ingredients/
│   ├── Components/
│   │   ├── IngredientForm.razor [modify]
│   │   └── IngredientGrid.razor [modify]
│   ├── CreateIngredient/CreateIngredientView.razor [modify]
│   ├── EditIngredient/EditIngredientView.razor [modify]
│   ├── IngredientsResources.cs [modify]
│   ├── IngredientsResources.resx [modify]
│   ├── IngredientsView.razor [modify]
│   ├── Models/
│   │   ├── IngredientCategoryOption.cs [modify]
│   │   ├── IngredientFormModel.cs [modify]
│   │   ├── IngredientFormModelValidator.cs [modify]
│   │   └── IngredientViewModel.cs [modify]
│   └── Shared/
│       ├── IngredientApiClient.cs [modify]
│       ├── IngredientApiContracts.cs [modify]
│       └── IngredientRoutes.cs [modify]
└── Products/
    ├── Components/
    │   ├── ProductForm.razor [modify]
    │   ├── ProductRetirementConfirmation.razor [create]
    │   ├── ProductTable.razor [modify]
    │   └── ProductDeleteConfirmation.razor [delete]
    ├── CreateProduct/CreateProductView.razor [modify]
    ├── EditProduct/EditProductView.razor [modify]
    ├── Models/
    │   ├── ProductFormModel.cs [modify]
    │   ├── ProductFormModelValidator.cs [modify]
    │   └── ProductViewModel.cs [modify]
    ├── ProductsResources.cs [modify]
    ├── ProductsResources.resx [modify]
    ├── ProductsView.razor [modify]
    └── Shared/
        ├── ProductApiContracts.cs [modify]
        ├── ProductsApiClient.cs [modify]
        └── ProductsRoutes.cs [modify]
MyHomeRamen.ArchitectureTests/ConventionTests/BlazorSliceConventionsTests.cs [modify]
```

## 3. Company catalog

### 3.1 API contracts
- Keep UI-owned transport contracts in each aggregate's `Shared/`; do not reference backend/domain types.
- Remove `Price` from catalog product request and response contracts. Represent product name, description, taxonomy, base ingredient IDs, permitted custom ingredient IDs, and server-supported active/retired status.
- Keep ingredient and product categories as taxonomy. Category ordering remains catalog behavior and must not drive customer menu sections.
- Product image is a UI placeholder only: use `ImageModel` with no URL and do not add image upload or image fields to API contracts.
- Login workflow owns restaurant context. Catalog clients read the selected restaurant from `RestaurantContextService` and send it as `X-Restaurant-Id`; do not add Company IDs, a Company selector, or a separate catalog context flow.
- Confirm ingredient catalog contracts do not expose restaurant extra prices; restaurant ingredient pricing belongs to Phase 3.

### 3.2 API client
- Extend existing `IngredientApiClient`, `ProductsApiClient`, and `CategoryApiClient`; preserve `ApiService.ExecuteAsync`, `ApiResult<T>`, and `EnsureApiSuccessAsync` behavior.
- Use `DELETE /api/menu/products/{id}` for product retirement; backend changes status to retired and returns 204. Do not add reactivation UI.
- Retain existing typed-client registration in Menu `DependencyInjection.cs`; no duplicate clients.

### 3.3 UI models
- Update ingredient, product, and category view/form models and validators to match the final catalog contracts.
- Remove product price fields, validation, and formatting. Keep category taxonomy fields and product base/custom ingredient relationships.
- Model returned product status and allowed actions; do not infer allowed operations from localized labels.

### 3.4 Components
- Update product forms and tables for ingredient configuration and server-supported status, without price fields. Render image placeholders through the existing `ImageModel`/`ImageDisplay`; do not offer upload.
- Replace delete confirmation with localized retirement confirmation that explains restaurant offers become hidden without rewriting them.
- Keep shared forms/components presentation-only; forms accept validation errors and error messages and emit callbacks.
- Preserve category create/reorder behavior as taxonomy management. Keep components responsive and expose actions without hover-only controls.

### 3.5 View
- Keep action-specific create/edit product and ingredient roots responsible for loading options, validation, API calls, errors, and navigation.
- Update product entry root to show status and allowed actions, retire products through confirmation, and reload authoritative catalog state after success.
- Preserve separate load/action errors; submit failures must not discard loaded content or form values.
- Provide explicit empty/error states and translation completeness where catalog text is localized business content.

### 3.6 Page
- Keep existing catalog routes and authorization policies; pages remain thin route/layout/title wrappers.
- Keep category routes for ingredient and product taxonomy. Do not route customer menu navigation through catalog categories.

### 3.7 Resources
- Update product and ingredient labels, validation, empty/error states, status, retirement confirmation, and success messages.
- Keep standard localized keys (`PageTitle`, `Loading`, `LoadError`, `ValidationFailure`, `UnexpectedError`) and remove obsolete delete/price text.

## 4. Decisions needed
- No blockers. Keep existing API/Blazor error behavior for this phase; track cross-cutting error-contract refactor in `../api-error-contract-refactor-draft.md`.
