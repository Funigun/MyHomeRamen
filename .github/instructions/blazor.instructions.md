---
description : Guidelines for the Blazor UI Presentation layer
applyTo: '*MyHomeRamen.Blazor*.razor, *MyHomeRamen.Blazor*.cs'
---

# Blazor Layer Instructions

## Overview
The Blazor frontend (`MyHomeRamen.Blazor` for Server, `MyHomeRamen.Blazor.Client` for WASM) provides the UI for restaurant management. 
It follows the Modular Monolith and Vertical Slice architecture patterns mirroring the backend modules.

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

## Pages
- Every routable `@page` component must wrap its main content in a `<MudPaper>` to ensure consistent visual structure, widths, and margins across the application.
- **Keep pages thin**: Pages are responsible for orchestration (loading data, handling state, routing callbacks). They should not embed complex or repeated markup directly. Extract any non-trivial UI block — especially one that appears more than once — into a dedicated component in the feature's `Components/` folder.
- **Compose with components**: A page's `@body` should read as a flat list of high-level component calls (`<CreateCategoryForm>`, `<CategoryTable>`, etc.), making the intent clear at a glance.

## Models & Forms
- **Decouple API DTOs from UI Models**: Do not bind UI forms directly to backend API DTOs.
  - Use **API DTOs** (e.g., `CreateProductRequest`) strictly for backend network payloads. Place in the feature action folder (e.g., `CreateProduct/CreateProductRequest.cs`).
  - Use **UI Models** for housing UI state — never expose API response records directly to components that need interactivity. Place in `Components/`.
- **UI Model naming convention**: Two distinct model types exist per domain entity — use consistent naming to make the intent explicit:
  - **`{Entity}FormModel`** — mutable model bound to `{Entity}Form.razor` via MudBlazor `MudForm`. Holds editable fields, exposes `ToCreateRequest()` / `ToEditRequest()` mapping methods, and provides a static `FromResponse(GetXxxByIdResponse)` factory for pre-filling edit forms.
  - **`{Entity}TableModel`** — flat read-only display model bound to `{Entity}Table.razor`. Populated from the corresponding `GetXxxForManageResponse` API DTO via a static `FromResponse(GetXxxForManageResponse)` factory. Contains only the fields the table needs to render — never exposes navigation properties or mutable state.
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
