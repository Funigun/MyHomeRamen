# Task Implementation Plan — Blazor: Create Product

- **Date**: 2025-07-14
- **Feature**: CreateProduct — Blazor frontend for creating a new product (Admin only)
- **Backend Endpoint**: `POST /api/menu.products` (group `Menu.Products`, requires `Admin` role)
- **Backend Request**: `CreateProductRequest(Name, Description, Price, CategoryId, IngredientIds)`
- **Backend Response**: `CreateProductResponse(Id)` — returns `201 Created`

---

## 1) Existing Primitive Validators (in `MyHomeRamen.Common.Contracts`)

The following validators already exist and **must be reused** in the Blazor `ProductValidator`:

| Validator | File | Rules |
|---|---|---|
| `ProductNameValidator` | `Common.Contracts/Menu/Products/ProductNameValidator.cs` | NotEmpty, MinLength(15), MaxLength(100) |
| `ProductDescriptionValidator` | `Common.Contracts/Menu/Products/ProductDescriptionValidator.cs` | NotEmpty, MinLength(50), MaxLength(500) |
| `ProductPriceValidator` | `Common.Contracts/Menu/Products/ProductPriceValidator.cs` | GreaterThanOrEqualTo(0.5), LessThanOrEqualTo(100.0) |

---

## 2) Create Feature Folder Structure

```
MyHomeRamen.Blazor/MyHomeRamen.Blazor/
├── Common/
│   └── Models/
│       └── FormMode.cs                            # Shared form mode enum (Create/View/Edit)
└── Features/C
    └── Menu/
        ├── Common/
        │   └── Services/
        │       └── MenuApiClient.cs               # Typed HttpClient for Menu API module
        └── Products/
            ├── Components/
            │   ├── ProductForm.razor               # Unified form (Create/Edit/View via FormMode)
            │   ├── ProductModel.cs                 # UI form model (decoupled from API DTO)
            │   └── ProductValidator.cs             # FluentValidation validator for ProductModel
            └── CreateProduct/
                ├── CreateProductPage.razor          # Routable page @page "/menu/products/create"
                └── CreateProductRequest.cs          # API request DTO for POST
```

---

## 3) Create `FormMode` Enum (shared)

**File**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Common/Models/FormMode.cs`

```csharp
namespace MyHomeRamen.Blazor.Common.Models;

public enum FormMode
{
    Create = 0,
    View = 1,
    Edit = 2
}
```

> This enum does not exist yet in the codebase. It's a shared concern used by all unified forms per the Blazor layer instructions.

---

## 4) Create API Request DTO

**File**: `Features/Menu/Products/CreateProduct/CreateProductRequest.cs`

```csharp
namespace MyHomeRamen.Blazor.Features.Menu.Products.CreateProduct;

public sealed record CreateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds);
```

> Mirrors the backend `CreateProductRequest` but lives in the Blazor project. This is the network payload DTO — **not** used for form binding.

---

## 5) Create UI Model

**File**: `Features/Menu/Products/Components/ProductModel.cs`

```csharp
using MyHomeRamen.Blazor.Features.Menu.Products.CreateProduct;

namespace MyHomeRamen.Blazor.Features.Menu.Products.Components;

public sealed class ProductModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public ICollection<Guid> IngredientIds { get; set; } = [];

    public CreateProductRequest ToCreateRequest()
    {
        return new CreateProductRequest(
            Name,
            string.IsNullOrWhiteSpace(Description) ? null : Description,
            Price,
            CategoryId,
            IngredientIds);
    }
}
```

> Follows the `SignUpModel` → `SignUpRequest` mapping pattern already established in the codebase. Manual mapping via `ToCreateRequest()`.

---

## 6) Create Validator

**File**: `Features/Menu/Products/Components/ProductValidator.cs`

```csharp
using FluentValidation;
using MyHomeRamen.Common.Contracts.Menu.Products;

namespace MyHomeRamen.Blazor.Features.Menu.Products.Components;

public sealed class ProductValidator : AbstractValidator<ProductModel>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name)
            .SetValidator(new ProductNameValidator());

        RuleFor(x => x.Description)
            .SetValidator(new ProductDescriptionValidator());

        RuleFor(x => x.Price)
            .SetValidator(new ProductPriceValidator());

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Please select a category.");

        RuleFor(x => x.IngredientIds)
            .NotEmpty()
            .WithMessage("Please select at least one ingredient.");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        FluentValidation.Results.ValidationResult result = await ValidateAsync(
            ValidationContext<ProductModel>.CreateWithOptions(
                (ProductModel)model,
                x => x.IncludeProperties(propertyName)));

        return result.IsValid ? [] : result.Errors.Select(e => e.ErrorMessage);
    };
}
```

> Reuses all three primitive validators from `MyHomeRamen.Common.Contracts`. Follows the `SignUpValidator` pattern with `ValidateValue` delegate for MudBlazor integration.

---

## 7) Create `MenuApiClient` (Module-Specific HTTP Client)

**File**: `Features/Menu/Common/Services/MenuApiClient.cs`

```csharp
using MyHomeRamen.Blazor.Features.Menu.Products.CreateProduct;

namespace MyHomeRamen.Blazor.Features.Menu.Common.Services;

public sealed class MenuApiClient(HttpClient httpClient)
{
    public async Task<Guid> CreateProductAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/menu.products", request, ct);
        response.EnsureSuccessStatusCode();

        CreateProductResponse? result = await response.Content.ReadFromJsonAsync<CreateProductResponse>(ct);
        return result?.Id ?? throw new InvalidOperationException("Failed to deserialize product creation response.");
    }
}

public sealed record CreateProductResponse(Guid Id);
```

> Follows the `EmployeeApiClient` / `CustomerAccountApiClient` pattern. Uses `AdminAuthHeaderHandler` since CreateProduct requires `Admin` role.

---

## 8) Register `MenuApiClient` in DI

**File**: `Presentation/ApiDependencyInjection.cs`

Add registration targeting the main API service (not Identity API):

```csharp
services.AddHttpClient<MenuApiClient>(client =>
    {
        client.BaseAddress = new Uri($"https+http://{infrastructurePrefix}-api");
    }
).AddHttpMessageHandler<AdminAuthHeaderHandler>();
```

> The API project is registered as `{prefix}-api` in Aspire (see `ProjectRegistrationExtensions.AddApiService`). Uses `AdminAuthHeaderHandler` since this is an Admin-only endpoint.

---

## 9) Create `ProductForm.razor` (Unified Form Component)

**File**: `Features/Menu/Products/Components/ProductForm.razor`

Key aspects:
- Accepts `[Parameter] public FormMode Mode { get; set; }` to control Create/View/Edit behavior
- Uses `MudForm` with `ProductValidator.ValidateValue` for validation
- Fields: `MudTextField` for Name, `MudTextField` (multiline) for Description, `MudNumericField` for Price, `MudSelect` for CategoryId, `MudSelect` (multi) for IngredientIds
- Shows submit button only in Create/Edit modes
- Disables all fields in View mode
- Uses `_isBusy` / `_errorMessage` pattern from `SignUpForm.razor`
- Exposes `[Parameter] public EventCallback<Guid> OnSuccess { get; set; }` to notify parent page
- Injects `MenuApiClient` and calls `CreateProductAsync` on submission
- Keeps submission logic in `@code {}` block (no code-behind needed per instructions)

**Note**: CategoryId and IngredientIds dropdowns will need data sources. For the initial implementation, these can accept `[Parameter]` collections:
- `[Parameter] public IEnumerable<CategoryOption> Categories { get; set; }`
- `[Parameter] public IEnumerable<IngredientOption> Ingredients { get; set; }`

These option types can be simple records defined alongside the form or in the Common models folder:

```csharp
public sealed record CategoryOption(Guid Id, string Name);
public sealed record IngredientOption(Guid Id, string Name);
```

> **Dependency note**: Category and Ingredient listing endpoints may not exist yet on the backend. The plan assumes they will be available. If not, stub data or a TODO should be left for those `MenuApiClient` methods.

---

## 10) Create `CreateProductPage.razor`

**File**: `Features/Menu/Products/CreateProduct/CreateProductPage.razor`

```razor
@page "/menu/products/create"
@attribute [Authorize(Roles = "admin")]

@using MyHomeRamen.Blazor.Common.Models
@using MyHomeRamen.Blazor.Features.Menu.Products.Components

<PageTitle>Create Product</PageTitle>

<MudContainer MaxWidth="MaxWidth.Medium">
    <MudText Typo="Typo.h4" Class="mb-4">Create New Product</MudText>
    <ProductForm Mode="FormMode.Create"
                 Categories="_categories"
                 Ingredients="_ingredients"
                 OnSuccess="HandleSuccess" />
</MudContainer>

@code {
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private IEnumerable<CategoryOption> _categories = [];
    private IEnumerable<IngredientOption> _ingredients = [];

    // TODO: Load categories and ingredients from MenuApiClient on OnInitializedAsync

    private void HandleSuccess(Guid productId)
    {
        Navigation.NavigateTo($"/menu/products/{productId}");
    }
}
```

> Simple page — no code-behind needed. Role-gated to `admin`. On success navigates to the product detail page (future feature).

---

## 11) Summary of Files to Create/Modify

### New Files:

| # | File | Purpose |
|---|---|---|
| 1 | `Common/Models/FormMode.cs` | Shared form mode enum |
| 2 | `Features/Menu/Common/Services/MenuApiClient.cs` | Typed HTTP client for Menu API |
| 3 | `Features/Menu/Products/Components/ProductModel.cs` | UI form model |
| 4 | `Features/Menu/Products/Components/ProductValidator.cs` | FluentValidation validator |
| 5 | `Features/Menu/Products/Components/ProductForm.razor` | Unified product form |
| 6 | `Features/Menu/Products/CreateProduct/CreateProductRequest.cs` | API request DTO |
| 7 | `Features/Menu/Products/CreateProduct/CreateProductPage.razor` | Routable page |

### Modified Files:

| # | File | Change |
|---|---|---|
| 1 | `Presentation/ApiDependencyInjection.cs` | Register `MenuApiClient` with `AdminAuthHeaderHandler` |

---

## 12) Reference Patterns Used

| Pattern | Reference File |
|---|---|
| UI Model with manual mapping | `Features/Account/Components/SignUpModel.cs` |
| FluentValidation with `ValidateValue` | `Features/Account/Components/SignUpValidator.cs` |
| MudForm with validation binding | `Features/Account/Components/SignUpForm.razor` |
| Typed HttpClient (module service) | `Features/Admin/Employees/EmployeeApiClient.cs` |
| DI registration with auth handler | `Presentation/ApiDependencyInjection.cs` |
| Simple page wrapping form component | `Features/Account/SignUp/SignUpPage.razor` |

---

## 13) Decisions

1. **Category & Ingredient listing endpoints**: Do not exist and are **out of scope**. The `ProductForm` should accept `[Parameter]` collections for categories and ingredients. The `CreateProductPage` should leave a TODO for loading this data once the endpoints are available.
2. **Navigation**: Nav menu does **not** require updates.
3. **Product listing page**: **Not** part of scope.
4. **FormMode for future use**: The `ProductForm` is designed as a unified form. When Edit/View features are added later, the same form component will be reused with different `FormMode` values.
