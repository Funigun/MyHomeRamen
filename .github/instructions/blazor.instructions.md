---
description : Guidelines for the Blazor UI Presentation layer
applyTo: '*MyHomeRamen.Blazor*.razor, *MyHomeRamen.Blazor*.cs'
---

# Blazor Layer Instructions

## Overview
The Blazor frontend (`MyHomeRamen.Blazor` for Server, `MyHomeRamen.Blazor.Client` for WASM) provides the UI for restaurant management. 
It follows the Modular Monolith and Vertical Slice architecture patterns mirroring the backend modules.

## Server vs WASM Best Practices
- **Server Project (`MyHomeRamen.Blazor`)**: Best for initial page loads, SEO, prerendering, and components requiring direct, secure access to backend resources or heavy computations.
- **WASM Project (`MyHomeRamen.Blazor.Client`)**: Best for highly interactive UI features, offloading rendering logic to the client, and rich client-side responsiveness.

## Architecture & Patterns
- Follow **Vertical Slice Architecture** for features (e.g., placing `.razor`, `.razor.cs`, Form Models, and Validators in the same feature folder).
- Integrate with API via typed `HttpClient` services (see **HttpClient Registration** below).
- For state management during prerendering, rely on the `[PersistState]` attribute to securely hand off data from the Server to the WASM client.
- Handle authentication via auth handlers (see **Authentication Handlers** below).

## Guidelines
- Use **MudBlazor** as the core component library. Do not use Bootstrap.
- **Performance**: Implement purely presentational features with standard HTML rather than writing unnecessary Blazor components. Save Blazor components for stateful, interactive elements.
- **Validation**: Secure forms using `FluentValidation`. Validators should aggressively reuse primitive base validators from `MyHomeRamen.Common.Contracts`.
- Implement role-based UI rendering.

## Services
- **Common Services**: Belong in a shared Common folder. These include services like `MessageService` (wrapping MudBlazor dialogs/snackbars) or `BaseHttpClient` (handling auth headers, token refresh, and common API interactions).
- **Module-Specific Services**: Belong strictly within their respective module folders to maintain boundaries. This includes strongly typed HTTP clients (e.g., `CatalogHttpClient`) that leverage the central `BaseHttpClient`.

### HttpClient Registration
All typed HttpClients are registered in `Presentation/ApiDependencyInjection.cs`.

- **Aspire service naming**: Backend services are named `{infrastructurePrefix}-api` (main API) and `{infrastructurePrefix}-identity-api` (Identity API). The `infrastructurePrefix` is passed into `AddApiServices(services, infrastructurePrefix)`.
- Register each module's HttpClient targeting the correct backend service:
  ```csharp
  services.AddHttpClient<MenuApiClient>(client =>
      {
          client.BaseAddress = new Uri($"https+http://{infrastructurePrefix}-api");
      }
  ).AddHttpMessageHandler<AdminAuthHeaderHandler>();
  ```
- Reference: `Presentation/ApiDependencyInjection.cs`

### Authentication Handlers
Two auth handlers exist — choose based on the endpoint's authorization policy:
- **`AuthHeaderHandler`**: For endpoints accessible by authenticated users (customers, employees).
- **`AdminAuthHeaderHandler`**: For endpoints restricted to the `Admin` role.

### API Route Convention
Backend endpoints are grouped using `IGroupEndpoint.GroupName` (e.g., `Menu.Products`). The route is derived as:
```
/api/{GroupName.ToLowerInvariant()}
```
Example: group `Menu.Products` → route `/api/menu.products`. Use this when building HttpClient request URLs.

## Pages
- Every routable `@page` component must wrap its main content in a `<MudPaper>` to ensure consistent visual structure, widths, and margins across the application.
- **Keep pages thin**: Pages are responsible for orchestration (loading data, handling state, routing callbacks). They should not embed complex or repeated markup directly. Extract any non-trivial UI block — especially one that appears more than once — into a dedicated component in the feature's `Components/` folder.
- **Compose with components**: A page's `@body` should read as a flat list of high-level component calls (`<CreateCategoryForm>`, `<CategoryTable>`, etc.), making the intent clear at a glance.
- Example structure:
  ```razor
  @page "/my-feature"

  <PageTitle>My Feature</PageTitle>

  <MudPaper Elevation="3" Class="pa-6">
      <MudText Typo="Typo.h4" Class="mb-6">My Feature</MudText>

      <MyFeatureForm OnSuccess="OnCreated" />

      <MudDivider Class="my-6" />

      <MyFeatureTable Title="Items" Items="_items" IsLoading="_isLoading" />
  </MudPaper>

  ```

## Models & Forms
- **Decouple API DTOs from UI Models**: Do not bind UI forms directly to backend API DTOs.
  - Use **API DTOs** (e.g., `CreateProductRequest`) strictly for backend network payloads. Place in the feature action folder (e.g., `CreateProduct/CreateProductRequest.cs`).
  - Use **UI Models** for housing UI state — never expose API response records directly to components that need interactivity. Place in `Components/`.
- **UI Model naming convention**: Two distinct model types exist per domain entity — use consistent naming to make the intent explicit:
  - **`{Entity}FormModel`** — mutable model bound to `{Entity}Form.razor` via MudBlazor `MudForm`. Holds editable fields, exposes `ToCreateRequest()` / `ToEditRequest()` mapping methods, and provides a static `FromResponse(GetXxxByIdResponse)` factory for pre-filling edit forms.
  - **`{Entity}TableModel`** — flat read-only display model bound to `{Entity}Table.razor`. Populated from the corresponding `GetXxxForManageResponse` API DTO via a static `FromResponse(GetXxxForManageResponse)` factory. Contains only the fields the table needs to render — never exposes navigation properties or mutable state.

  ```csharp
  // ✅ Form model — mutable, validated, maps to API payload
  public sealed class ProductFormModel
  {
      public string Name { get; set; } = string.Empty;
      public decimal Price { get; set; }
      public IEnumerable<Guid> CategoryIds { get; set; } = [];

      public CreateProductRequest ToCreateRequest() => new(Name, Price, CategoryIds);
      public static ProductFormModel FromResponse(GetProductByIdResponse r) => new() { Name = r.Name, Price = r.Price, CategoryIds = r.CategoryIds };
  }

  // ✅ Table model — flat, read-only, populated from list response
  public sealed class ProductTableModel
  {
      public Guid Id { get; init; }
      public string Name { get; init; } = string.Empty;
      public decimal Price { get; init; }

      public static ProductTableModel FromResponse(GetProductsForManageResponse r) => new() { Id = r.Id, Name = r.Name, Price = r.Price };
  }

  // ❌ API DTO used directly as component parameter
  [Parameter] public List<GetProductsForManageResponse> Items { get; set; } = [];

  // ✅ Table model used as component parameter
  [Parameter] public List<ProductTableModel> Items { get; set; } = [];
  ```
- **Manual mapping**: UI Models expose a `ToXxxRequest()` method to map to the API DTO. No AutoMapper or Mapster.
  ```csharp
  public CreateProductRequest ToCreateRequest() => new(Name, Description, Price, CategoryId, IngredientIds);
  ```
- **Unified Forms**: Keep UI logic reusable by creating a single `{Feature}Form.razor` component that handles Create, View, and Update scenarios. Control this state with the `FormMode` Enum (`Common/Models/FormMode.cs`) passed as `[Parameter] public FormMode Mode { get; set; }`.
- **Code-behind vs Form Model**:
  - For **form components** (`{Feature}Form.razor`): a dedicated `{Feature}Model.cs` + `{Feature}Validator.cs` is sufficient. No `.razor.cs` code-behind is needed — keep submission logic in the `@code` block.
  - For **page components** (`{Feature}Page.razor`): extract to a `.razor.cs` code-behind only when the page-level logic is complex enough to justify it (e.g., multiple API calls, conditional rendering logic, lifecycle orchestration). Simple pages should also stay inline.

### Paged Lists
- Use the shared `PageState` sealed record (`Common/Models/PageState.cs`) instead of three separate `int` parameters (`TotalCount`, `PageSize`, `CurrentPage`).
- Table components accept a single `[Parameter] public PageState Paging { get; set; } = PageState.Default()` plus `EventCallback<int> OnPageChanged`.
- Pages manage paging state with `with`-expressions for immutable updates — never mutate `PageState` fields directly.
- Use `MudPagination` bound to `Paging.TotalPages` / `Paging.CurrentPage`; render it conditionally only when `Paging.TotalPages > 1`.

```csharp
// ✅ PageState — shared immutable paging model in Common/Models/PageState.cs
public sealed record PageState
{
    public int CurrentPage { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public int TotalCount { get; init; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 1;
    public static PageState Default(int pageSize = 10) => new() { PageSize = pageSize };
}

// ✅ Table component — single PageState parameter replaces three loose ints
[Parameter] public PageState Paging { get; set; } = PageState.Default();
[Parameter] public EventCallback<int> OnPageChanged { get; set; }

// ✅ Markup — MudPagination bound to PageState
@if (Paging.TotalPages > 1)
{
    <MudPagination Count="Paging.TotalPages"
                   Selected="Paging.CurrentPage"
                   SelectedChanged="OnPageChanged"
                   ShowFirstButton="true"
                   ShowLastButton="true" />
}

// ✅ Page — initialise and update PageState with with-expressions
private PageState _ingredientsPaging = PageState.Default();

// After loading data:
_ingredientsPaging = _ingredientsPaging with { TotalCount = response.TotalCount };

// On page navigation callback:
private async Task OnIngredientPageChangedAsync(int page)
{
    _ingredientsPaging = _ingredientsPaging with { CurrentPage = page };
    await LoadIngredientsAsync();
}

// ❌ Three separate loose parameters — use PageState instead
[Parameter] public int TotalCount { get; set; }
[Parameter] public int PageSize { get; set; }
[Parameter] public int CurrentPage { get; set; }
```

### MudBlazor Validation Integration
Validators must expose a `ValidateValue` delegate for MudBlazor's `MudForm` binding:
```csharp
public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
{
    FluentValidation.Results.ValidationResult result = await ValidateAsync(
        ValidationContext<TModel>.CreateWithOptions(
            (TModel)model,
            x => x.IncludeProperties(propertyName)));
    return result.IsValid ? [] : result.Errors.Select(e => e.ErrorMessage);
};
```
Bind in the form via `<MudForm Validation="_validator.ValidateValue">`.

### Form Submission Pattern
All forms follow a consistent submission pattern with busy state and error handling:
```csharp
private MudForm _form = default!;
private bool _isBusy;
private string? _errorMessage;

private async Task SubmitAsync()
{
    await _form.Validate();
    if (!_form.IsValid) return;

    _isBusy = true;
    _errorMessage = null;
    try
    {
        // call API client
        await OnSuccess.InvokeAsync(result);
    }
    catch (HttpRequestException)
    {
        _errorMessage = "Operation failed. Please try again.";
    }
    finally
    {
        _isBusy = false;
    }
}
```

## Structure Example
```
|-- Presentation/
|   -- ApiDependencyInjection.cs          # All typed HttpClient registrations
|   -- AuthenticationDependencyInjection.cs
|   -- Authentication/
|       -- AuthHeaderHandler.cs           # For authenticated user endpoints
|       -- AdminAuthHeaderHandler.cs      # For admin-only endpoints
|-- Components/ (Global components like MainLayout, NavMenu)
|-- Common/
|   -- Services/ (e.g., BaseHttpClient, MessageService)
|   -- Models/ (e.g. FormMode enum)
|-- Features/
|   -- {ModuleName}/
|       -- Common/
|           -- Services/ (e.g. {ModuleName}ApiClient.cs, {ModuleName}NavigationService.cs)
|           -- Models/ (shared models across module features e.g. Option models, enums)
|           -- Constants/ (e.g. Role names)
|       -- {FeatureName}/
|           -- Components/
|               -- {FeatureName}Form.razor
|               -- {FeatureName}FormModel.cs   # mutable model for form binding (Create/Edit)
|               -- {FeatureName}TableModel.cs  # read-only display model for table/list
|               -- {FeatureName}Validator.cs
|           -- Requests/
|               -- {FeatureName1}Request.razor
|               -- {FeatureName2}Request.razor 
|           -- Responses/
|               -- {FeatureName1}Response.razor
|               -- {FeatureName2}Response.razor 
|           -- {ActionName}Page.razor
|           -- {ActionName}Page.razor.cs  (only if complex)
```

## Reference Implementations
When building a new feature, use these canonical files as patterns:

| Pattern | Reference File |
|---|---|
| UI Model with `ToXxxRequest()` mapping | #file:'MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/Components/ProductModel.cs' |
| Validator with `ValidateValue` delegate | #file:'MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/Components/ProductValidator.cs' |
| MudForm with validation + submission | #file:'MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/Components/ProductForm.razor' |
| API Request DTO | #file:'MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/Requests/CreateProductRequest.cs' |
| API Response DTO | #file:'MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/Responses/CreateProductResponse.cs' |
| Reusable display component (list/table) | #file:'MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Categories/Components/CategoryTable.razor' |
| Typed HttpClient (module service) | #file:'MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Admin/Employees/EmployeeApiClient.cs' |
| HttpClient DI registration | #file:'MyHomeRamen.Blazor/MyHomeRamen.Blazor/Presentation/ApiDependencyInjection.cs' |
| Navigation service | #file:'MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Common/Services/MenuNavigationService.cs' |
| Navigation service DI registration | #file:'MyHomeRamen.Blazor/MyHomeRamen.Blazor/Presentation/NavigationDependencyInjection.cs' |
| Simple page wrapping a form component | #file:'MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/CreateProductPage.razor' |
| Thin page composing multiple components + PageState management | #file:'MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Ingredients/IngredientsManagementPage.razor' |
| Shared paging state model (`PageState`) | #file:'MyHomeRamen.Blazor/MyHomeRamen.Blazor/Common/Models/PageState.cs' |
| Paged table component with `MudPagination` | #file:'MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Ingredients/Components/IngredientTable.razor' |

## Navigation

- **Never inject `NavigationManager` directly into page components.** Use a module-scoped `{Module}NavigationService` instead.
- Each module has a single `{Module}NavigationService` in `Features/{Module}/Common/Services/`.
- The service exposes a **static nested `Routes` class** (for use in `@page` directives and `href` attributes) and **imperative navigation methods** (for use after async actions).
- Register all navigation services in `Presentation/NavigationDependencyInjection.cs` and call `services.AddNavigationServices()` from `Program.cs`.
- Inject the service with `@inject` in Razor components.

```csharp
// Features/{Module}/Common/Services/{Module}NavigationService.cs
public sealed class MenuNavigationService(NavigationManager navigation)
{
    public static class Routes
    {
        public const string List = "/menu/products";
        public const string Create = "/menu/products/create";

        public static string Detail(Guid id) => $"/menu/products/{id}";
        public static string Edit(Guid id) => $"/menu/products/{id}/edit";
    }

    public void ToList() => navigation.NavigateTo(Routes.List);
    public void ToCreate() => navigation.NavigateTo(Routes.Create);
    public void ToDetail(Guid id) => navigation.NavigateTo(Routes.Detail(id));
    public void ToEdit(Guid id) => navigation.NavigateTo(Routes.Edit(id));
}
```

Usage in a page:
```razor
@inject MenuNavigationService MenuNavigation

@code {
    private void HandleSuccess(Guid productId) => MenuNavigation.ToDetail(productId);
}
```

Usage in markup (static route):
```razor
<MudNavLink Href="@MenuNavigationService.Routes.List">Products</MudNavLink>
```
