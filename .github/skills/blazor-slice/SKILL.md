---
name: blazor-slice
description: Create or update a Blazor UI slice (page, view, form, models, API client, routes, resources) from templates according to project standards.
---

# Blazor Slice

Create Blazor UI slice from `templates/`. Copy template, replace tokens, adjust. Do not invent new structure.

Rules: `.github/instructions/blazor.instructions.md`. Reference code: `Features/Restaurants`, `Features/Restaurants/Companies/CompanyDetails`.

## Tokens

| Token | Meaning | Example |
|---|---|---|
| `{Module}` | Feature module | `Restaurants` |
| `{Aggregate}` | Aggregate, singular | `Restaurant` |
| `{Aggregates}` | Aggregate, plural | `Restaurants` |
| `{Slice}` | Slice folder = action + aggregate | `CreateRestaurant` |
| `{Policy}` | `AdminSectionConstants` policy | `RestaurantCreation` |

## Templates

| Template | Target |
|---|---|
| `Routes.cs.tmpl` | `{Aggregate}/Shared/{Aggregate}Routes.cs` |
| `ApiClient.cs.tmpl` | `{Aggregate}/Shared/{Aggregate}ApiClient.cs` |
| `Resources.cs.tmpl` + `Resources.resx.tmpl` | `{Slice}Resources.cs/.resx` or aggregate root |
| `Page.razor.tmpl` | `{Slice}/{Slice}Page.razor` |
| `LoadView.razor.tmpl` | entry or details slice root (load + show) |
| `SubmitView.razor.tmpl` | create/edit slice root (form submit) |
| `Form.razor.tmpl` | `{Slice}/Components/{Slice}Form.razor` |
| `FormModel.cs.tmpl` | `{Slice}/Models/{Slice}FormModel.cs` + validator |

## Steps

1. Reuse check: existing client, routes, resources in aggregate? extend, do not duplicate.
2. Folders + files by blazor.instructions.md structure.
3. Routes + API contracts (no backend types).
4. API client from `ApiClient.cs.tmpl`; register typed client in module `DependencyInjection.cs`.
5. Models: ViewModel (`FromResponse`), FormModel (`ToRequest`), validator. Use an aggregate-level `{Aggregate}FormModel` and validator when Create/Read/Edit intentionally share fields and validation; otherwise use action-specific models.
6. Resources: standard keys + slice keys.
7. Form component: params + callbacks only. Use one aggregate-level `{Aggregate}Form` for shared Create/Read/Edit presentation modes instead of generating duplicate action forms; adapt `Form.razor.tmpl` if no shared form exists. Mode controls presentation only; slice root owns API operation, authorization, and navigation. Use the action-specific form template when fields or validation differ.
8. View: pick `LoadView` or `SubmitView`.
9. Page last: route, authorize, layout, title only.
10. `dotnet build MyHomeRamen.Blazor\MyHomeRamen.Blazor`. Fix errors.

## Invariants

- no `var`; primary constructors
- views: no HTTP try/catch, no `ApiService`
- load error and submit error: separate `ApiErrorState`
- `isSubmitting` reset in `finally`
- string params with `@`
- all text from resources; standard keys: `PageTitle`, `Loading`, `LoadError`, `ValidationFailure`, `UnexpectedError`
- no empty or stub pages
