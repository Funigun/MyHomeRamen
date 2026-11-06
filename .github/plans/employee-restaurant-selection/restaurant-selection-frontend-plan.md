# Frontend Plan: Restaurants - Company and Restaurant Identification

## 1. Problem

Customers and employees need different restaurant-selection flows: customers browse active restaurant options, while employees must select only restaurants authorized for their work. Post-login routing must bypass employee selection for CompanyOwner and preserve validated restaurant context through refreshes and restaurant-scoped requests.

## 2. File tree

```text
MyHomeRamen.Blazor/MyHomeRamen.Blazor/
├── Components/
│   └── Layout/Admin/
│       └── CompanyOwnerLayout.razor [create]
├── Features/
│   ├── Admin/
│   │   ├── Common/Services/
│   │   │   └── AdminNavigationService.cs [modify]
│   │   └── CompanyOwnerHome/
│   │       ├── CompanyOwnerHomePage.razor [create]
│   │       ├── CompanyOwnerHomeView.razor [create]
│   │       ├── CompanyOwnerHomeResources.cs [create]
│   │       └── CompanyOwnerHomeResources.resx [create]
│   ├── Home/
│   │   ├── HomePage.razor [modify]
│   │   ├── HomeView.razor [create]
│   │   ├── HomeResources.cs [create]
│   │   └── HomeResources.resx [create]
│   ├── Menu/RestaurantMenu/
│   │   ├── RestaurantMenuPage.razor [modify]
│   │   ├── RestaurantMenuView.razor [modify]
│   │   └── Shared/RestaurantMenuRoutes.cs [create]
│   └── Restaurants/
│       ├── DependencyInjection.cs [modify]
│       ├── Common/Services/RestaurantContextService.cs [create]
│       └── Restaurants/
│           ├── Components/RestaurantOptionsSelector.razor [create]
│           ├── Models/RestaurantOptionViewModel.cs [create]
│           ├── SelectRestaurant/
│           │   ├── SelectRestaurantPage.razor [create]
│           │   ├── SelectRestaurantView.razor [create]
│           │   ├── SelectRestaurantResources.cs [create]
│           │   └── SelectRestaurantResources.resx [create]
│           └── Shared/
│               ├── RestaurantApiClient.cs [modify]
│               ├── RestaurantApiContracts.cs [create]
│               └── RestaurantRoutes.cs [modify]
└── Infrastructure/Authentication/
    └── SignInSignOutRouteBuilderExtensions.cs [modify]
```

## 3. Employee/admin flow

### 3.1 API contracts
- Add UI transport contracts for `GET /api/restaurants/employee-options` with restaurant ID, name, street, city, ZIP code, latitude, and longitude. Do not reference feature/backend DTO types from Blazor.
- Reuse the current `employee-options` filtering for now: restaurants represented by resource-scoped permission grants. Do not add a new access-list contract for this flow; API operations remain responsible for checking their required permissions.
- Extend `GetMeQuery` with one server-derived destination for the current principal: CompanyOwner, Employee, Customer, or Guest. Do not use display names, client-provided role labels, or `CanViewAdminPanel` as the CompanyOwner discriminator.
- Keep employee/admin and Customer account modes separate; do not combine them on one account. The same person may use separate accounts. Guest browsing remains publicly available, independent of employee context. Backend must verify Guest provisioning, CompanyOwner/RestaurantAdmin permission boundaries, and public-browsing grants.

### 3.2 API client
- Extend existing typed `RestaurantApiClient`; add employee-options call returning `ApiResult<T>` through `ApiService.ExecuteAsync`.
- Keep existing client registration and auth handlers; do not add a duplicate restaurant client.

### 3.3 UI models
- Map transport contracts into `RestaurantOptionViewModel` with a `FromResponse` mapping boundary.
- Save employee restaurant ID in `ProtectedSessionStorage`, tied to authenticated identity. It survives refresh and is discarded when the tab session ends. Revalidate it against current employee options on entry/reload; stored ID is context, never authorization.

### 3.4 Components
- Add shared `RestaurantOptionsSelector` for option name/address and selection callback. Component receives options and localized labels; it does not fetch, authorize, persist, or navigate.
- Render selector options as custom rows with alternating image-placeholder and city/address columns. Stack each option vertically on narrow screens.

### 3.5 View
- `SelectRestaurantView` is post-sign-in entry root: resolve authoritative destination, load employee options, and own error, empty, auto-selection, and navigation states.
- After sign-in, send CompanyOwner directly to `/company-owner` with dedicated owner layout. For employees: save and continue automatically for one option; show selector for multiple.
- If an employee has no active authorized restaurant, allow sign-in but deny employee/admin entry and retain guest browsing. Do not show a sign-out action. A CompanyOwner with `Restaurants.Manage` can enter the owner area and activate a restaurant.
- Revalidate saved employee context after refresh and fail closed if missing, inactive, revoked, or no longer authorized. No in-app employee restaurant switcher; logout and new sign-in required to select another.

### 3.6 Page
- `SelectRestaurantPage` is authenticated at `/admin/select-restaurant`, uses a neutral/authenticated layout rather than the restaurant-scoped admin layout, localizes title, and renders `SelectRestaurantView`.
- Keep `/admin-panel` as the employee landing route. Its current page is an empty stub using `AdminLayout`.
- Add a thin CompanyOwner home page at `/company-owner` using `CompanyOwnerLayout`; preserve route-specific restaurant IDs for CompanyOwner restaurant-management and menu-preview links. Company-level operations must not depend on selected restaurant.

### 3.7 Resources
- Add localized loading, no-access, load-error, selection, and page-title keys. Pass localized labels into shared selector.

## 4. Customer flow

### 4.1 API contracts
- Add a separate UI transport contract for anonymous `GET /api/restaurants/options`; map the same ID/name/address/coordinate fields without sharing the employee response type.
- Customer options contain active restaurants only. Do not use employee options or infer access from authentication.
- Guests and authenticated Customers share this Company-wide browsing behavior; do not create restaurant-specific copies of either role.

### 4.2 API client
- Add customer-options call to existing `RestaurantApiClient`, returning `ApiResult<T>`.

### 4.3 UI models
- Map customer contracts into `RestaurantOptionViewModel`; retain restaurant ID as browsing context.
- Keep customer browsing context independent from employee context and any existing basket's restaurant.

### 4.4 Components
- Reuse `RestaurantOptionsSelector` presentation with customer-specific loading and selection lifecycle.

### 4.5 View
- Refactor `HomePage` into thin page plus `HomeView`; `HomeView` loads active options, shows loading/empty/failure states, and starts browsing on selection.
- Navigate to ID-free `/menu` and resolve the selected restaurant from customer browsing context; retain context through refreshes and direct links. If context is missing, return to the restaurant picker rather than guessing a restaurant.
- Changing browsed restaurant must not silently clear or transfer an existing basket; replacement behavior remains deferred to Shopping Cart plan.
- Pass browsing context on restaurant-scoped menu and add-to-basket requests; checkout continues using restaurant identity saved with basket. Do not loosen existing basket sign-in requirements.

### 4.6 Page
- Preserve current `/` and `/home` entry routes and render `HomeView`. Do not add a Company selector.
- Keep customer `/menu` route ID-free and pass customer context into existing menu view; keep route/page markup thin. CompanyOwner live-menu preview uses a separate owner route containing the target restaurant ID, not a global restaurant switcher.
- Keep selector responsive and keyboard/touch accessible; use same workflow on desktop and phone.

### 4.7 Resources
- Localize picker title, labels, loading, empty, failure, and selection text in Home resources.

## 5. Shared restaurant context

- Add `RestaurantContextService` for identity-bound employee context and customer browsing context, stored separately in `ProtectedSessionStorage`. Both survive refresh and are discarded when the tab session ends.
- Wire sign-in entry to the restaurant-entry flow; bind employee context to authenticated identity and clear it on logout/identity change. Revalidate on entry; never use persistence as authorization.
- Do not create global CompanyOwner restaurant context. Resolve the target restaurant from each owner route that operates on or previews a restaurant.
- Continue constructing `X-Restaurant-Id` on each restaurant-scoped `HttpRequestMessage` using its explicit restaurant ID. Never mutate shared `HttpClient` default headers; keep documented headerless requests headerless.
- Pass employee or customer context to their respective restaurant-scoped operations; use owner route ID for owner restaurant operations. Preserve basket's own restaurant ID for basket/checkout operations; API rechecks authorization and context on every request.
- Keep current headerless exceptions headerless and never let locally persisted context grant access.
- Keep guest/customer and employee flows separate in routing and data source while reusing only selector presentation.
