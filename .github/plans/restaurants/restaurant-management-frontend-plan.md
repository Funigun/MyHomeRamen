# Frontend Plan: Restaurants - Restaurant Management

## 1. Problem
Build restaurant management UI for company owners. The restaurants view must use `CompanyOwnerPanelHeader`, show a responsive restaurant-card grid with Add, AssignManager, and edit actions, and provide navigation from restaurant General details to WorkingHours and ClosingPeriods.

WorkingHours and ClosingPeriods pages are navigation placeholders only. Their detailed workflows require separate frontend plans.

## 2. Files to create / modify
| Action | Module | Aggregate | Feature Name | Route | Page |
|--------|--------|-----------|--------------|-------|------|
| Create | Restaurants | Restaurant | Get available restaurants |  | `MyHomeRamen.Blazor/Features/Restaurants/Restaurants/Pages/RestaurantsPage.razor` |
| Create | Restaurants | Restaurant | Get available restaurants |  | `MyHomeRamen.Blazor/Features/Restaurants/Restaurants/RestaurantsView.razor` |
| Create | Restaurants | Restaurant | Get available restaurants |  | `MyHomeRamen.Blazor/Features/Restaurants/Restaurants/Components/RestaurantGrid.razor` |
| Create | Restaurants | Restaurant | Get available restaurants / Assign manager |  | Restaurant card and AssignManager UI components beside the restaurants view |
| Modify | Restaurants | Restaurant | Create restaurant | `api/restaurants/restaurants` (API route) | `MyHomeRamen.Blazor/Features/Restaurants/Restaurants/Pages/CreateRestaurantPage.razor` |
| Modify | Restaurants | Restaurant | Edit restaurant / Get restaurant by ID | `api/restaurants/restaurants/{restaurantId}` (API route) | `MyHomeRamen.Blazor/Features/Restaurants/Restaurants/Pages/EditRestaurantPage.razor` |
| Create | Restaurants | Restaurant | Create restaurant / Edit restaurant |  | Create/edit view, form model, and validation files beside the respective slices |
| Create | Restaurants | Restaurant | Get restaurant by ID / Get available restaurants / Assign manager |  | Restaurant API contracts and resource client in `Restaurants/Shared` |
| Create | Restaurants | Restaurant | General restaurant details |  | General page/view for Company details, Contact Details, and Bank Account |
| Create | Restaurants | Restaurant | WorkingHours placeholder |  | Blank WorkingHours page/view |
| Create | Restaurants | Restaurant | ClosingPeriods placeholder |  | Blank ClosingPeriods page/view |
| Create | Restaurants | Restaurant | Restaurant section navigation |  | Shared restaurant header component and route builders |
| Modify | Restaurants | Company | Shared owner navigation |  | `MyHomeRamen.Blazor/Features/Restaurants/Companies/Shared/CompanyOwnerPanelHeader.razor` |
| Create or Modify | Restaurants | Restaurant | All restaurant UI features |  | Module DI registration and localized resources, following existing project patterns |

## 3. Restaurant Management

### 3.1 Folder structure
- Keep `Restaurants/Restaurants/Pages/CreateRestaurantPage.razor` as the create route and `EditRestaurantPage.razor` as the edit route; pages define route, authorization, title, and render slice roots only.
- Add `RestaurantsPage.razor` and `RestaurantsView.razor` for the company-owner restaurants view. The view owns loading, action state, API orchestration, errors, and navigation.
- Add `RestaurantGrid.razor` and a restaurant-card component under `Restaurants/Restaurants/Components`; components receive models and callbacks only.
- Add separate General, WorkingHours, and ClosingPeriods page/view folders or slices. General contains Company details, Contact Details, and Bank Account sections. WorkingHours and ClosingPeriods render blank placeholders until their detailed plans are approved.
- Add a shared restaurant navigation header under `Restaurants/Restaurants/Shared`; it navigates between General, WorkingHours, and ClosingPeriods without loading data or calling APIs.
- Keep company-level navigation in `Companies/Shared/CompanyOwnerPanelHeader.razor`; wire its Restaurants action to the restaurants view and preserve Company Details and Social Media navigation.

### 3.2 API contracts
- Map available-restaurant responses to UI data containing ID, name, active state, Address, and ContactDetails.
- Map restaurant details to UI data containing ID, name, active state, Address, ContactDetails, BankAccount, and Manager details.
- Use separate create/update request contracts for restaurant name and any confirmed edit fields; never bind Razor forms directly to transport contracts.
- Add an AssignManager request contract based on the confirmed manager-selection payload. The preview does not define its API route or request shape; leave this as an implementation decision requiring confirmation.
- Keep API URL construction and status/error mapping in the resource client. Map validation failures, forbidden responses, not-found responses, and unexpected failures explicitly.
- Preserve preview API routes for create and get-by-ID. The preview leaves list and assign routes blank; do not invent them in this plan.

### 3.3 API client
- Add a resource-oriented `RestaurantsApiClient` in the restaurant shared area with methods for available restaurants, restaurant details, create, edit, and manager assignment.
- Use typed `HttpClient`, existing serialization/error infrastructure, and module DI registration.
- Keep API clients separate from company and unrelated module clients.
- Confirm list, update, and AssignManager endpoint routes before implementation.

### 3.4 UI models
- Create a restaurant list model with ID, name, active state, Address, and ContactDetails.
- Create a restaurant details model with manager information and BankAccount in addition to list fields.
- Use separate create and edit form models. Create must validate required name, uniqueness feedback, and the confirmed maximum length of 200.
- Use a manager-selection model for AssignManager; exact fields and source of available managers require confirmation.
- Map API responses into UI models and UI form models into request contracts at the client/view boundary.
- Track loading, empty, forbidden, error, submitting, and success states explicitly. Keep server validation errors attached to the relevant form or operation.

### 3.5 Components
- `CompanyOwnerPanelHeader` renders owner-level navigation: Company Details, Social Media, and Restaurants. Replace the current non-navigating buttons with project-standard links/callbacks and correct the Social Media label while wiring routes.
- The restaurants view renders section line `Restaurants` with an Add button above the grid.
- `RestaurantGrid` renders responsive cards, one card per restaurant, with touch-friendly Edit and AssignManager actions that remain usable on narrow screens and do not depend on hover.
- Add opens `CreateRestaurantPage.razor`; Edit opens `EditRestaurantPage.razor` for the selected restaurant.
- AssignManager opens the confirmed assignment workflow and reports completion/cancellation through callbacks. It must not load data or navigate independently.
- The restaurant header exposes General, WorkingHours, and ClosingPeriods navigation. General exposes Company details, Contact Details, and Bank Account subsections.
- Placeholder WorkingHours and ClosingPeriods views show only localized section/page context and an intentional not-yet-implemented state; do not add scheduling behavior.

### 3.6 View
- `RestaurantsView` renders `CompanyOwnerPanelHeader`, the `Restaurants` heading with Add, then loading/error/empty state or `RestaurantGrid`.
- Load available restaurants through the resource client and map results to list models ordered as provided by the API.
- Add navigates to the create page. Edit navigates to the edit page with the selected restaurant ID. AssignManager coordinates manager selection, submission, success feedback, and list refresh/update.
- Preserve list state on recoverable errors, prevent duplicate submissions, and show explicit forbidden, not-found, validation, and general-error states.
- Create and edit slice roots load required data, validate forms, call the appropriate client method, handle success navigation, and surface API errors without silent fallback.
- General loads restaurant details and renders Company details, Contact Details, and Bank Account. WorkingHours and ClosingPeriods remain blank placeholder slices.
- Localize page titles, headings, buttons, labels, card content, loading/empty/error/success text, validation messages, and placeholder text through the project resource pattern.

### 3.7 Page
- Add a routable restaurants view page. The preview leaves the UI route unspecified; confirm the browser route before implementation rather than deriving one from the API route.
- Keep create and edit as separate routable pages using the existing `CreateRestaurantPage.razor` and `EditRestaurantPage.razor` files. Do not combine workflows behind a mode parameter.
- Use a parameterized edit route containing restaurant ID, aligned with the get-by-ID API operation. Confirm exact browser route and route-builder naming before implementation.
- Add separate routes for General, WorkingHours, and ClosingPeriods under the selected restaurant context. Exact route structure remains a decision requiring confirmation.
- Apply restaurant-management permissions to page authorization and keep backend authorization authoritative. The preview confirms `Restaurants.Create` for create and `Restaurant.Manage` / `Restaurants.Manage` for management operations.
- Keep localized `PageTitle` values on pages and render only the corresponding slice root.
