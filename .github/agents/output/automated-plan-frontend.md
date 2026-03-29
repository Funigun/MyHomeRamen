# Feature CreateIngredient plan:
- **Date**: 2025-01-17
- **Feature**: CreateIngredient

11) Create frontend feature structure
    - Create `MyHomeRamen.Blazor\MyHomeRamen.Blazor\Features\Menu\Ingredients\` directory
    - Create `Common\Models\IngredientOption.cs` if needed for dropdowns
    - Create `Components\IngredientForm.razor`, `IngredientModel.cs`, `IngredientValidator.cs`
    - Create `IngredientsIndex\IngredientsIndexPage.razor`
    - Create `CreateIngredient\CreateIngredientPage.razor`, `CreateIngredientRequest.cs`

12) Create or update API communication services and API Response model
    - Update `MenuApiClient.cs`: add `CreateIngredientAsync(CreateIngredientRequest request)` method returning `Guid`
    - Add `CreateIngredientResponse` record in `MenuApiClient.cs`

13) Create or update models, DTOs and mappings
    - `IngredientModel`: properties for Name, Description, Price, SelectedCategoryIds (List<Guid>)
    - `IngredientModel.ToCreateRequest()`: maps to `CreateIngredientRequest`
    - `IngredientValidator`: extends `BaseValidator<IngredientModel>`, reuses `IngredientNameValidator`, etc., validates categories not empty

14) Create or update Blazor components and pages
    - `IngredientForm.razor`: MudForm with fields for Name, Description, Price, MudSelect for Categories (load from GetCategoriesForDropdown)
    - `IngredientsIndexPage.razor`: page at `/admin/ingredients`, list ingredients (placeholder for now)
    - `CreateIngredientPage.razor`: page at `/admin/ingredients/create`, hosts `IngredientForm` with `Mode = FormMode.Create`, loads categories on init

15) Create Unit tests for Blazor components and services
    - Unit tests are not required for this feature.
