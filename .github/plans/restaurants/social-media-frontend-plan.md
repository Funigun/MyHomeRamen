# Frontend Plan: Restaurants - Social Media Management

## 1. Problem
Build company social media management UI at `/restaurants/companies/social-media`. Page must show company owner navigation, load company-owned social media, support add/edit/delete actions, and use responsive cards with two cards per row where space permits.

The repository already contains an initial `Management` scaffold. Keep this structure for this implementation so it can be compared with two other company pages before deciding whether to move to action-oriented slices.

## 2. Files to create / modify
| Action | Module | Aggregate | Feature Name | Route | Page |
|--------|--------|-----------|--------------|-------|------|
| Modify | Restaurants | Company | GetSocialMediaForManage | `/restaurants/companies/social-media` | `MyHomeRamen.Blazor/Features/Restaurants/Companies/SocialMedia/Management/SocialMediaPage.razor` |
| Modify | Restaurants | Company | GetSocialMediaForManage / CreateSocialMedia / UpdateSocialMedia / DeleteSocialMedia |  | `MyHomeRamen.Blazor/Features/Restaurants/Companies/SocialMedia/Management/SocialMediaView.razor` |
| Modify | Restaurants | Company | GetSocialMediaForManage |  | `MyHomeRamen.Blazor/Features/Restaurants/Companies/SocialMedia/Management/Components/SocialMediaGrid.razor` |
| Modify | Restaurants | Company | CreateSocialMedia / UpdateSocialMedia |  | `MyHomeRamen.Blazor/Features/Restaurants/Companies/SocialMedia/Management/Components/SocialMediaDialog.razor` |
| Modify | Restaurants | Company | Shared owner navigation |  | `MyHomeRamen.Blazor/Features/Restaurants/Companies/Shared/CompanyOwnerPanelHeader.razor` |
| Create | Restaurants | Company | GetSocialMediaForManage / CreateSocialMedia / UpdateSocialMedia / DeleteSocialMedia |  | Social media UI models, form model, and validation files beside the management slice |
| Create or Modify | Restaurants | Company | GetSocialMediaForManage / CreateSocialMedia / UpdateSocialMedia / DeleteSocialMedia |  | Company social media API contracts and resource client in the existing company shared/API-client location |
| Create or Modify | Restaurants | Company | Social media management |  | Module dependency-injection registration and localized resources, only where existing registration/localization patterns require changes |

## 3. Social Media Management

### 3.1 Folder structure
- Keep `SocialMediaPage.razor` as the routable page with route and authorization.
- Keep `SocialMediaView.razor` as the initial slice root; it owns loading, API orchestration, dialog state, mutation results, and error handling.
- Keep `SocialMediaGrid.razor` under `Management/Components`; it renders supplied social media cards and raises edit/delete callbacks without loading data or calling APIs.
- Keep `SocialMediaDialog.razor` under `Management/Components`; use it for both add and edit presentation through explicit state supplied by `SocialMediaView`.
- Reuse `Companies/Shared/CompanyOwnerPanelHeader.razor` in the page shell/view. Header must remain a presentation/navigation component and must not load company data or call APIs.
- Do not split create, edit, and delete into separate action folders yet. Reassess this structure after implementing the two related company pages.

### 3.2 API contracts
- Represent management responses with SocialMedia ID, `Name`, `LogoUrl`, and `Url`.
- Represent create/update requests with `Name`, `LogoUrl`, and `Url`; keep transport contracts separate from Razor-bound UI models.
- Support:
  - `GET .../company/{id}/social-media/manage` for management loading.
  - `POST .../company/{id}/social-media` for add; treat empty `201 Created` as success.
  - `PUT .../company/{id}/social-media/{socialMediaId}` for edit; treat `204 No Content` as success.
  - `DELETE .../company/{id}/social-media/{socialMediaId}` for delete; treat `204 No Content` as success.
- Keep API URL construction and status/error mapping inside the resource client.
- Confirm how current frontend resolves company ID before implementation. The preview uses `{id}`, while the requested UI route has no route parameter; do not invent an ID source or fallback.
- Map validation failures to dialog/form validation, `403` to an explicit permission state, and unexpected failures to the existing application error pattern.

### 3.3 API client
- Add or extend a company social media resource client with methods for loading, creating, updating, and deleting company social media.
- Keep this client separate from unrelated module resources; do not create one catch-all HTTP client.
- Use typed `HttpClient`, existing serialization/error infrastructure, and module-level DI registration.
- Verify the confirmed company-ID source and backend URL shape before finalizing request methods.

### 3.4 UI models
- Map management transport responses to a display model containing ID, name, logo URL, and URL.
- Use a separate add/edit form model containing `Name`, `LogoUrl`, and `Url`; never bind the dialog directly to API DTOs.
- Provide explicit mappings from response to edit model and from valid form model to create/update requests.
- Validate fields using the existing SocialMedia rules. Keep backend-confirmed constraints aligned; identify any unspecified client-side constraints for confirmation rather than inventing them.
- Track dialog mode, selected social media ID, loading/submission state, delete confirmation state, and operation errors in `SocialMediaView`.

### 3.5 Components
- `CompanyOwnerPanelHeader` remains at the top of the owner panel and exposes navigation for Company Details, Social Media, and Restaurants. Use the existing component as the shared transitional header; correct label typo/route behavior only as part of wiring navigation.
- `SocialMediaGrid` renders responsive cards, ideally two cards per row on desktop and a single stacked card on narrow screens.
- Each card displays available logo/name/URL information and always exposes touch-friendly Edit and Delete actions; actions must not depend on hover.
- `SocialMediaGrid` receives items and callbacks for edit/delete. It must not fetch data, open dialogs, or decide authorization.
- `SocialMediaDialog` receives open state, add/edit mode, form model, validation state, and callbacks. It renders fields for name, logo URL, and URL, with responsive/full-screen behavior suitable for phones.
- The section heading and Add button belong to the view shell, above the grid. Add opens `SocialMediaDialog` in create mode; card Edit opens it in edit mode.

### 3.6 View
- `SocialMediaView` renders the page shell in this order: `CompanyOwnerPanelHeader`, social media section title with Add button, loading/error/empty state or `SocialMediaGrid`.
- Load management data on initialization through the resource client and map it to UI models.
- Open a blank form for Add; populate form from selected card for Edit.
- Validate before submit, prevent duplicate submissions, call the correct create/update operation, close the dialog on success, and refresh or update the displayed list.
- Ask for delete confirmation before calling delete. On successful deletion, remove or refresh the item and show the existing success feedback pattern.
- Preserve current list and form input when recoverable validation/API errors occur; show explicit forbidden and general error states.
- Keep layout responsive: stack section controls on narrow screens, keep cards readable, and make dialogs usable on phones.
- Localize page heading, section title, labels, buttons, dialog title, loading/empty/error/success text, and validation messages through the project resource pattern.

### 3.7 Page
- Keep `@page "/restaurants/companies/social-media"` on `SocialMediaPage.razor`.
- Keep `[Authorize(Policy = PermissionConstants.CompanySocialMediaView)]` on the page for initial read access. Add/edit/delete controls must also respect the edit permission and backend authorization; do not rely on UI hiding for security.
- Keep localized `PageTitle` on the page and render `SocialMediaView` as the workflow root.
- Do not add `{id}` to the UI route. Resolve company context through the established company-owner flow once confirmed.
- Keep API route details separate from the browser route; the backend preview route remains company-ID based.
