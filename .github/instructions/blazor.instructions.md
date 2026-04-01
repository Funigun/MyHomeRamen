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
  - Use **UI Models** (e.g., `ProductModel`) for housing UI state, reactive properties, and `FluentValidation` validator bindings. Place in `Components/`.
- **Manual mapping**: UI Models expose a `ToXxxRequest()` method to map to the API DTO. No AutoMapper or Mapster.
  ```csharp
  public CreateProductRequest ToCreateRequest() => new(Name, Description, Price, CategoryId, IngredientIds);
  ```
- **Unified Forms**: Keep UI logic reusable by creating a single `{Feature}Form.razor` component that handles Create, View, and Update scenarios. Control this state with the `FormMode` Enum (`Common/Models/FormMode.cs`) passed as `[Parameter] public FormMode Mode { get; set; }`.
- **Code-behind vs Form Model**:
  - For **form components** (`{Feature}Form.razor`): a dedicated `{Feature}Model.cs` + `{Feature}Validator.cs` is sufficient. No `.razor.cs` code-behind is needed — keep submission logic in the `@code` block.
  - For **page components** (`{Feature}Page.razor`): extract to a `.razor.cs` code-behind only when the page-level logic is complex enough to justify it (e.g., multiple API calls, conditional rendering logic, lifecycle orchestration). Simple pages should also stay inline.

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
|           -- Models/ (shared models across module features)
|           -- Constants/ (e.g. Role names)
|       -- {FeatureName}/
|           -- Components/
|               -- {FeatureName}Form.razor
|               -- {FeatureName}Model.cs
|               -- {FeatureName}Validator.cs
|           -- {ActionName}/ (e.g. CreateProduct, EditProduct)
|               -- {ActionName}Page.razor
|               -- {ActionName}Page.razor.cs  (only if complex)
|               -- {ActionName}Request.cs     (API DTO)
```

## Reference Implementations
When building a new feature, use these canonical files as patterns:

| Pattern | Reference File |
|---|---|
| UI Model with `ToXxxRequest()` mapping | `Features/Account/Components/SignUpModel.cs` |
| Validator with `ValidateValue` delegate | `Features/Account/Components/SignUpValidator.cs` |
| MudForm with validation + submission | `Features/Account/Components/SignUpForm.razor` |
| Reusable display component (list/table) | `Features/Menu/Categories/Components/CategoryTable.razor` |
| Typed HttpClient (module service) | `Features/Admin/Employees/EmployeeApiClient.cs` |
| HttpClient DI registration | `Presentation/ApiDependencyInjection.cs` |
| Navigation service | `Features/Menu/Common/Services/MenuNavigationService.cs` |
| Navigation service DI registration | `Presentation/NavigationDependencyInjection.cs` |
| Simple page wrapping a form component | `Features/Account/SignUp/SignUpPage.razor` |
| Thin page composing multiple components | `Features/Menu/Categories/CategoriesIndex/CategoriesIndexPage.razor` |

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
