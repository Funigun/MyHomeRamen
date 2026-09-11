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
- Pages define only route, layout, authorization and act as container for the slice main component.
- Slice main component orchestrates workflow: loading data, api calls, validation, and handling user interactions, can be split into sub-components for visual/responsibility separation.
- Prefer action-oriented slices such as `ListEmployees`, `ViewEmployee`, `CreateEmployee`, `EditEmployee`, and `DeleteEmployee`.
- Each slice owns its route, page state, API orchestration, validation, loading state, error handling, authorization, and post-action navigation.
- Share visual field groups and display components, not complete workflows.
- Shared components receive data and callbacks. They do not load data, call APIs, perform navigation, or decide which operation is executing.
- Define Dependency Injection registration extension per module.

Recommended structure:

```text
Features/
└── Admin/
    └── Employees/
        ├── ListEmployees/
        ├── ViewEmployee/
        ├── CreateEmployee/
        ├── EditEmployee/
        ├── ChangeEmployeeStatus/
        └── Shared/
        Role/
        ├── ListRoles/
        ├── CreateRole/
        ├── EditRole/
        └── Shared/
        DependencyInjection.cs
```

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

- Declare routable URLs with `@page` beside the page that owns the route.
- Do not duplicate every `@page` route in a module navigation service.
- Use direct `Href` or `NavigationManager.NavigateTo` for simple navigation.
- Use small static route builders for parameterized or reused URLs.
- Keep a navigation service only when it performs real workflow behavior such as preserving state, handling return URLs, or selecting a destination conditionally.
- Ensure route builders and `@page` declarations do not drift.

Example:

```csharp
public static class EmployeeRoutes
{
    public const string Index = "/admin/employees";
    public const string Create = "/admin/employees/create";

    public static string View(Guid id) => $"/admin/employees/{id}";
    public static string Edit(Guid id) => $"/admin/employees/{id}/edit";
}
```

## 5) Localization

- Use resource files for application-owned text: labels, buttons, navigation, validation, errors, titles, and empty states.
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