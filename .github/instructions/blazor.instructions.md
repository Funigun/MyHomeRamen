---
description: 'Instructions for Blazor UI architecture'
applyTo: '**/MyHomeRamen.Blazor/**/*.razor,**/MyHomeRamen.Blazor/**/*.cs,**/MyHomeRamen.Blazor.Client/**/*.razor,**/MyHomeRamen.Blazor.Client/**/*.cs'
---

# Blazor UI Instructions

## 1) Application direction

- Keep the primary UI as a Blazor Web App with Interactive Server rendering.
- Do not introduce WebAssembly migration work unless explicitly requested.
- Keep `MyHomeRamen.Blazor.Client` isolated from the server UI if it remains in the solution.
- Keep server-only concerns out of reusable UI components and client assemblies.

## 2) Vertical Slice Architecture

- Do not use single page/component for multiple workflows e.g. Create, View, Edit defined by page/form parameter.
- Organize UI by module, aggregate, and user action.
- Pages define route, layout, authorization, localized page title, and render the slice root component. Keep workflow markup out of pages.
- Slice root component owns the page shell and orchestrates workflow: loading data, api calls, validation, error handling, and user interactions. It can be split into sub-components for visual/responsibility separation.
- Prefer action-oriented slices such as `ListEmployees`, `ViewEmployee`, `CreateEmployee`, `EditEmployee`, and `DeleteEmployee`.
- Each slice owns its route, page state, API orchestration, validation, loading state, error handling, authorization, and post-action navigation.
- Share visual field groups and display components, not complete workflows or slice root components.
- Shared components receive data and callbacks. They do not load data, call APIs, perform navigation, or decide which operation is executing.
- Keep workflow roots beside their page within the feature slice. Use `Shared` only for reusable components, models, contracts, clients, and cross-slice UI primitives.
- Define Dependency Injection registration extension per module.

### Slice structure (reference: `Features/Restaurants`)

- One folder per action slice, named after the action: `CreateRestaurant`, `EditRestaurant`, `CompanyDetails`, `RegisterOwner`, `SocialMedia`.
- Entry slice of an aggregate = `{Aggregate}Page` + `{Aggregate}View` at the aggregate root (e.g. `RestaurantsPage`). It is the aggregate's entry page; it is not necessarily a list, so no `List{X}` naming rule. Its `Components/` and `Models/` sit beside it in the aggregate root.
- Folder name = slice name. Do not use generic names such as `Management`, `Pages`, `Models` at aggregate level for action slices.
- Every slice uses the same file set:

| File | Role |
|---|---|
| `{Slice}Page.razor` | Route, `[Authorize]`, layout, localized `PageTitle`, renders `{Slice}View`. Nothing else. |
| `{Slice}View.razor` | Slice root: loading, API calls, errors, navigation, state. |
| `Components/*.razor` | Dumb parts (forms, grids, dialogs). Parameters + `EventCallback` only. |
| `Models/*.cs` | UI ViewModels, FormModels, validators. Namespace `...{Slice}.Models`. |

- Only routable components end with `Page`. Everything beside the page is a slice root or part of it.
- Slice-local resources: `{Slice}Resources.cs` + `.resx` in the slice folder.
- Resources used by several slices of one aggregate (e.g. `RestaurantsResources`) live in the aggregate root.
- Name Form components `{Action}{Aggregate}Form`; models `{Action}{Aggregate}FormModel` and `...FormModelValidator`.
- A slice with one workflow gets no sub-folders besides `Components/` and `Models/`.

```text
Features/
└── {Module}/
    ├── DependencyInjection.cs               ← one per module
    ├── Shared/                              ← module-wide constants (permissions etc.)
    └── {Aggregate}/
        ├── {Aggregate}Page.razor            ← entry page of aggregate
        ├── {Aggregate}View.razor
        ├── {Aggregate}Resources.cs/.resx    ← shared by slices
        ├── Components/                      ← entry slice parts, e.g. {Aggregate}Grid.razor
        ├── Models/                          ← entry slice models, e.g. {Aggregate}ViewModel.cs
        ├── Shared/
        │   ├── {Aggregate}ApiClient.cs
        │   ├── {Aggregate}ApiContracts.cs
        │   └── {Aggregate}Routes.cs
        ├── Create{Aggregate}/
        │   ├── Create{Aggregate}Page.razor
        │   ├── Create{Aggregate}View.razor
        │   ├── Components/Create{Aggregate}Form.razor
        │   └── Models/Create{Aggregate}FormModel.cs (+ Validator)
        └── Edit{Aggregate}/ ...
```

- Aggregates made of independent slices (e.g. `Companies`) have no aggregate-root page; each slice is a folder (`CompanyDetails`, `RegisterOwner`, `SocialMedia`) and clients live in `Shared/`.
- Slices that cannot be a single action (e.g. details + inline edit) stay one slice named after the main view (`CompanyDetails`).
- Mixed list + dialog slices (`SocialMedia`) keep dialog in `Components/`; dialog gets callbacks, root calls API.

### API result handling

- Clients own HTTP: each client method wraps its call in `ApiService.ExecuteAsync(...)` and returns `ApiResult<T>` (`Success` / `Failure`). Non-success responses throw via `response.EnsureApiSuccessAsync(...)` (`ApiRequestException`). Views never catch HTTP exceptions and never call `ApiService`.
- Views hold errors in `ApiErrorState` (`Infrastructure/Api`): `Apply(result, badRequestMessage, unexpectedMessage)` returns true on failure. `ValidationErrors` and `Message` come from it.
- Keep load errors and submit errors in separate `ApiErrorState` instances. A submit error must not replace loaded content or discard the form.
- `GetDisplayableError()` returns null for success and for errors handled globally (403/5xx redirects); `ApiErrorState` already respects it. Do not copy `HandledGlobally` checks into views.
- Localize all error text via resources (`ValidationFailure`, `UnexpectedError`, `LoadError`). Never show `ApiError.Message`.
- Forms take `ValidationErrors` and `ErrorMessage` parameters; map field errors with `Error`/`ErrorText` by property name.
- Reset `isSubmitting` in `finally`.
- Map response to ViewModel inside `Success` branch; map FormModel to request with `ToRequest()`.
- Pass string fields to component parameters with `@` (`ErrorMessage="@_submitError.Message"`); without it Blazor passes the literal text.

### Enforced by architecture tests (`BlazorSliceConventionsTests`)

- Routable components end with `Page`.
- Pages do not inject `*ApiClient` or `ApiService`.
- Components in `Components/` do not inject `*ApiClient` or `ApiService`.
- Rules currently run for `Features.Restaurants` only. Extend `FeaturesNamespace` as modules are refactored.

## 3) API clients

- Do not single module-level http client for all module endpoints.
- Prefer resource clients such as `ProductsApiClient`, `RolesApiClient`, and `EmployeesApiClient`.
- Resource client may be used by multiple UI slices.
- Do not reference any backend services or domain.
- Do not use Api contracts or DTOs directly in UI.
- Create UI-specific view models and map to/from API contracts.

- Keep cross-resource orchestration in the page slice or a dedicated orchestration service.
- Use typed `HttpClient` registrations grouped by module through dependency-injection extension methods.
- Keep HTTP transport concerns in API clients and shared HTTP infrastructure.

## 4) Routes and navigation

- Declare each aggregate's URLs once in `{Aggregate}Routes` (static class in the aggregate `Shared/`), with `const` strings.
- Pages use `@attribute [Route({Aggregate}Routes.X)]` (Razor `@page` accepts only literals, so it cannot use constants). Parameterized routes use a `...Template` const plus a builder method.
- Nav menus, `Href` and `NavigationManager.NavigateTo` use the same constants. No hardcoded URL strings.
- Keep a navigation service only when it performs real workflow behavior such as preserving state, handling return URLs, or selecting a destination conditionally.

Example:

```csharp
public static class RestaurantRoutes
{
    public const string Index = "/restaurants/companies/restaurants";
    public const string Create = Index + "/create";
    public const string EditTemplate = Index + "/{Id:guid}/edit";

    public static string Edit(Guid id) => $"{Index}/{id}/edit";
}
```

## 5) Localization

- Use resource files for application-owned text: labels, buttons, navigation, validation, errors, titles, and empty states.
- Localize `PageTitle` in pages and headings, descriptions, loading/error states, and action text in slice root components.
- Keep restaurant or business content in the database with translation records.
- Use stable codes or IDs for roles, permissions, statuses, and other authorization data. Never use translated display names for authorization decisions.
- Resolve requested language and fallback language at the query/API boundary.
- Pages should receive localized view models. Components should not load translations themselves.
- Keep translation completeness and missing-translation handling explicit in administrative screens.
- Do not duplicate language-specific database columns such as `NameEn`, `NameDe`, and `NameFr`.

## 6) Responsive UI

- Design every slice for desktop and phone layouts from the start.
- Use one workflow and responsive presentation rather than separate mobile and desktop pages.
- Prefer cards or compact action menus for narrow-screen list views when tables become unusable.
- Use stacked forms, touch-friendly actions, and full-screen dialogs on phones.
- Avoid hover-only actions.
- Keep reusable responsive patterns in shared components and keep feature state in the slice.