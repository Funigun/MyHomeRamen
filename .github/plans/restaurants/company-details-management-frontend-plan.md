# Frontend Plan: Restaurants - Company Details Management

## 1. Problem
Build company details management page for currently supported single-company flow. Page must load company details, show editable company and business information when `CanEditCompanyDetails` is allowed, 
and preserve `CompanyOwnerPanelHeader` in `Companies/Shared` for reuse by social media and restaurant management pages.

The repository already contains an initial `Management` structure that differs from recommended action-oriented folders. Retain this scaffold for now: `CompanyManagementPage` owns route, authorization, and title; 
`CompanyDetailsView` remains slice root and owns state/orchestration. Revisit split into separate view/edit slices when final UI shape is decided.

## 2. Files to create / modify
| Action | Module | Aggregate | Feature Name | Route | Page |
|--------|--------|-----------|--------------|-------|------|
| Modify | Restaurants | Company | GetDetails | `/restaurants/companies/management` | `MyHomeRamen.Blazor/Features/Restaurants/Companies/Management/CompanyManagementPage.razor` |
| Modify | Restaurants | Company | UpdateDetails | `/restaurants/companies/management` | `MyHomeRamen.Blazor/Features/Restaurants/Companies/Management/CompanyManagementPage.razor` |
| Modify | Restaurants | Company | GetDetails / UpdateDetails |  | `MyHomeRamen.Blazor/Features/Restaurants/Companies/Management/CompanyDetailsView.razor` |
| Modify | Restaurants | Company | UpdateDetails |  | `MyHomeRamen.Blazor/Features/Restaurants/Companies/Management/CompanyDetailsForm.razor` |
| Modify | Restaurants | Company | GetDetails / UpdateDetails |  | `MyHomeRamen.Blazor/Features/Restaurants/Companies/Management/CompanyDetailsModel.cs` |
| Modify | Restaurants | Company | UpdateDetails |  | `MyHomeRamen.Blazor/Features/Restaurants/Companies/Management/CompanyDetailsModelValidator.cs` |
| Modify | Restaurants | Company | GetDetails / UpdateDetails |  | `MyHomeRamen.Blazor/Features/Restaurants/Companies/Shared/CompanyApiClient.cs` |
| Modify | Restaurants | Company | GetDetails / UpdateDetails |  | `MyHomeRamen.Blazor/Features/Restaurants/Companies/Shared/CompanyApiContracts.cs` |
| Modify | Restaurants | Company | Shared owner navigation |  | `MyHomeRamen.Blazor/Features/Restaurants/Companies/Shared/CompanyOwnerPanelHeader.razor` |
| Create | Restaurants | Company | GetDetails / UpdateDetails |  | Localized resource file(s) beside management slice, following project localization registration |

## 3. Company Details Management

### 3.1 Folder structure
- Keep existing `Companies/Management` scaffold for page, slice root, form, view model, and validator.
- Keep `CompanyOwnerPanelHeader.razor` under `Companies/Shared`; it must remain presentation/navigation UI reusable from company management, social media, and restaurant views.
- Do not move header into `Management` or make it load company data, call APIs, authorize actions, or perform workflow navigation internally.
- If final design confirms independent read and edit workflows, split them into action-oriented slices in a later plan; current implementation should not duplicate routes or workflows.

### 3.2 API contracts
- Keep transport DTOs in `Companies/Shared`, separate from UI models.
- Use existing `CompanyDetailsDto` mapping for `Id`, `Name`, `Description`, `LogoUrl`, `BusinessDetails.LegalName`, `BusinessDetails.TaxId`, and `AllowedActions`.
- Add update request contract for `Description`, `LogoUrl`, and `BusinessDetails` (`LegalName`, `TaxId`) using backend `BusinessDetailsForUpdateDto` shape.
- Add response/error handling for `200`/`204` success, `400` validation-policy failure, and `403` authorization failure. Do not expose transport DTOs to Razor components.
- Backend preview does not define endpoint routes. Confirm GetDetails and UpdateDetails routes before implementation; do not invent route constants in this plan.

### 3.3 API client
- Extend `CompanyApiClient` with resource methods for loading details and submitting updates.
- Keep HTTP URL construction, serialization, cancellation, status handling, and API exceptions inside client.
- Map API errors distinctly enough for slice root to show validation errors versus forbidden/general failure.
- Verify typed `HttpClient` and module DI registration already cover `CompanyApiClient`; add or update module registration only if missing.

### 3.4 UI models
- Keep a read/view model mapped from `CompanyDetailsDto`; do not bind form fields directly to transport DTOs.
- Add or adapt an editable form model containing description, nullable logo URL, legal name, and tax ID.
- Provide explicit mapping from loaded details to form model and from valid form model to update request.
- Represent `AllowedActions` by stable action key; show edit controls only when `CanEditCompanyDetails` is true.
- Complete `CompanyDetailsModelValidator` with client-side rules matching confirmed backend validation. Mark any rules not specified by backend as decisions requiring confirmation rather than inventing constraints.

### 3.5 Components
- `CompanyOwnerPanelHeader` renders reusable owner-panel navigation buttons/links and accepts only the data or callbacks/routes needed by its host. It must not select operations or own navigation state.
- `CompanyDetailsForm` receives form model, editability, validation state, and submit/cancel callbacks; it owns field layout and responsive presentation only.
- Add reusable field/display components only when needed by more than one company slice; keep company-details workflow state in `CompanyDetailsView`.
- Use stacked fields and touch-friendly actions on narrow screens; keep header navigation usable without hover-only behavior.

### 3.6 View
- `CompanyDetailsView` acts as slice root: load details during initialization, map to UI models, and render loading, success, forbidden, and general-error states explicitly.
- Render company identity/details and business details from loaded model.
- Render read-only state when `CanEditCompanyDetails` is false; render edit action/form when allowed.
- On submit, validate form, call update client method, handle `204`, show success state, and refresh or update displayed model without losing user context.
- Map `400` field/general validation failures into form validation; map `403` to an explicit permission message; surface unexpected failures through existing application error UI pattern.
- Prevent duplicate submissions, support cancellation where existing component patterns allow it, and preserve responsive layout.
- Localize heading, labels, buttons, loading, empty/error, validation, and success text through resource files. Keep database company content unlocalized by UI resources.

### 3.7 Page
- Keep `@page "/restaurants/companies/management"` on `CompanyManagementPage.razor` unless backend/product route decision changes.
- Keep `[Authorize(Policy = PermissionConstants.CompanyView)]` on page for initial read access; edit visibility and update authorization remain separate.
- Keep localized `PageTitle` on page and render only `CompanyDetailsView` as workflow root.
- Compose `CompanyOwnerPanelHeader` inside the slice/view or page shell according to final layout, without moving its file from `Shared`.
- Existing route is scaffold-derived because preview plan leaves Route blank; confirm route and navigation destinations for Company Details, Social Media, and Restaurants before final implementation.
