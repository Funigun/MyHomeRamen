# UI Refactor Plan: Company Catalog, Restaurants, Menus, and Shopping Cart

## Goal and scope

Implement the UI direction in [Domain Model Analysis](Domain%20Model%20Analysis.md) and [Domain Model Refactor Plan](Domain%20Model%20Refactor%20Plan.md). Keep one Company in the UI, multiple restaurants, Interactive Server rendering, and the existing server/client separation. The backend remains responsible for tenant isolation, restaurant access, prices, availability, and authorization.

Do not add a Company selector, Company-scoped roles, menu scheduling, price versioning, a custom ramen builder, taxes, discounts, fees, or refunds.

## Shared architecture and prerequisites

- Organize work under `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features` by module, aggregate, and action. Pages contain route, layout, authorization, localized title, and one view; views own loading, API calls, errors, interaction, and navigation. Components receive UI models and callbacks only.
- Keep separate create, read, and edit slice roots. Share an aggregate form only when fields and validation match; any mode controls presentation, not API orchestration. Use aggregate `Shared` folders for resource clients, contracts, and route constants; register typed clients in each module's DI extension.
- Map API contracts to UI-specific view models on success and form models to requests with `ToRequest()`. Use `ApiService.ExecuteAsync`, `ApiResult<T>`, `EnsureApiSuccessAsync`, and separate load/submit `ApiErrorState` instances. Localize errors; preserve loaded content and form input on submit failure; reset submitting state in `finally`.
- Centralize route templates and builders in aggregate route classes; use constants for pages and navigation. Extend `BlazorSliceConventionsTests` to each refactored module. Localize application text and show translation completeness where business content is edited. Support phone and desktop in the same workflow.
- Treat the source documents as the target, not proof that all endpoints exist. Backend work is in progress; confirm final routes, payloads, permissions, error codes, currency, and guest identity behavior against implemented contracts before wiring each slice. Do not invent success-shaped fallbacks.
- Current `CompanyContextMiddleware` requires `X-Restaurant-Id` for `/api/menu` and `/api/payments`, except card options. Resolve this bootstrap requirement before catalog or payment requests: use a validated restaurant context, not a Company ID chosen by the browser. Confirm the final contract for Shopping Cart and Orders as well. Attach context per request, never by mutating shared `HttpClient` default headers.

## 1. Company and restaurant identification

### Employee/admin flow

1. Keep `Features/Restaurants/Restaurants/RestaurantsPage` and `RestaurantsView` as the permission-filtered staff entry point. Use the existing available-restaurants query, not public restaurant options. Show only authorized restaurants and link each row to its restaurant-specific management route.
2. Replace the `EditRestaurantPage` stub with a thin page and `EditRestaurantView`; provide restaurant-specific navigation to offers, extra prices, and menu cards. Carry the route restaurant ID through each action, including direct links and refreshes.
3. Keep Company details and catalog navigation Company-level. Since current tenant middleware derives Company from a restaurant, define an explicit authorized restaurant context for catalog requests before loading them. If none is available, show a localized blocked/empty state rather than guessing an ID. Do not add a Company selector.
4. Evaluate existing admin-section claims and backend allowed actions using stable permission codes. Route context and visible navigation do not grant access: the API must revalidate the user, restaurant, and Company on every operation. Show access-denied or unavailable states for invalid, inactive, or unauthorized route IDs.

### Customer flow

1. Refactor `Features/Home/HomePage` into a thin page and a home view. Add a restaurant picker backed by the public active-restaurant options endpoint, separate from the staff query. Display name and available address; handle loading, no restaurants, and failure.
2. After selection, navigate to a restaurant-scoped menu route. Preserve the selected restaurant through menu, basket, checkout, and order requests, including deep links and reloads. Derive public Company context from the selected restaurant on the server; do not trust a client-supplied Company ID.
3. Keep staff and customer entry points distinct, not mutually exclusive: an employee can also browse as a customer. Avoid deciding access from display names or assuming that authentication alone means employee access. Support anonymous browsing under the backend's Guest permissions; keep existing sign-in requirements for basket actions unless the final API contract changes them.
4. Changing the browsed restaurant must not silently clear or transfer an existing basket. The basket remains tied to its own restaurant until the explicit replacement flow in section 5.

**Exit:** direct links, refreshes, and restaurant switching retain correct context; staff sees only permitted management entries; customers see only active restaurant options.

## 2. Company catalog: ingredients, products, and categories

1. Refactor `Features/Menu/Ingredients`, `Products`, and `Categories` as Company-catalog slices. Keep existing action-specific ingredient/product pages and views; split or retain category inline workflows according to the slice rules. Move HTTP contracts out of presentation components and map results into UI models.
2. Remove price from catalog product forms, models, tables, validators, and requests. Products contain name, description, image, taxonomy, base ingredients, permitted custom ingredients, and Active/Retired status. Base ingredients are included in the eventual offer price; permitted custom ingredients are not automatically available at every restaurant.
3. Keep ingredient and product categories as catalog taxonomy. Category ordering is catalog behavior, not menu section order. Do not use catalog categories as customer menu sections.
4. Replace product deletion with retirement UX and localized confirmation; explain that retirement hides all restaurant offers without rewriting them. Reflect server-supported status and permitted actions in list/detail views. Do not silently add reactivation if the API does not support it.
5. Update resource clients and UI models for tenant-scoped catalog requests and final DTOs. Include localized validation, empty/error states, responsive lists and forms, and API-enforced permission handling.

**Dependencies:** backend Phase 2 Company-scoped catalog and updated product/category/ingredient contracts; authorized restaurant context from section 1.

**Exit:** catalog screens contain no prices, categories remain taxonomy, and retirement cannot be confused with deleting an individual restaurant offer.

## 3. Restaurant product offers and ingredient prices

1. Add restaurant-scoped offer entry and action slices under `Features/Menu/Offers`, with a resource client and route constants. List catalog products with their restaurant offer status and price; provide create, price-edit, and retire actions through separate action roots or explicit inline workflows.
2. Create at most one offer per restaurant/product; validate price `> 0`, show currency from the Company/API contract, and surface duplicate or cross-Company failures. Retirement affects only that restaurant's offer; retiring a catalog product hides its offers everywhere. Do not imply retiring and recreating an offer is supported unless the API permits it.
3. Add restaurant-scoped extra-price slices under `Features/Menu/RestaurantIngredientPrices`. Show priced and unpriced catalog ingredients; allow setting or editing a price `>= 0`, including zero. Removing a price disables that ingredient as an extra in this restaurant; it does not delete the catalog ingredient or remove a base ingredient.
4. Explain that offer price is shared across every card listing that offer and price changes apply immediately. Only product-configured custom ingredients with a price at this restaurant are selectable as extras.
5. Gate offer admin actions by `Offer.Manage` plus the applicable `Add`, `Edit`, or `Delete` permission; use the corresponding `IngredientPrice` permissions for extra prices. Evaluate restaurant-scoped grants and server-returned allowed actions; the backend remains authoritative.

**Dependencies:** backend Phases 3-4, including scoped list/detail and mutation contracts. Existing in-progress command files do not establish that all management reads and UI-required allowed-action data are available; identify and deliver missing contracts before completing these slices.

**Exit:** two restaurants can price the same catalog product differently; free extras work, unpriced extras are unavailable, and a manager cannot operate on another restaurant by changing the route.

## 4. Restaurant menu cards and customer menu

### Management

1. Add `Features/Menu/MenuCards` with a restaurant-scoped entry page/view and action slices for card create, view, edit, and delete or retirement as supported by the final API. Cards have a name and active flag; a restaurant may have multiple cards.
2. Provide card-scoped workflows for sections and listings: create/edit/remove sections, reorder sections, add/remove restaurant offers, and reorder listings. Use ordered UI models, preserve server order after reload, reject duplicate offer placement within one section, and permit an offer in different sections or cards when supported.
3. Show that listings control placement only; edit prices in offer management. Filter choices to eligible offers of the route restaurant. Show unavailable/retired offers and conflicting changes using explicit API states; do not silently drop unsaved edits.
4. Gate card admin operations by `MenuCard.Manage` plus the relevant CRUD permission, scoped to the restaurant. Ensure direct routes receive the same server checks as navigation.

### Customer menu

1. Refactor `Features/Menu/RestaurantMenuPage` into a thin routed page and view. Replace `GetMenuCategories` and `GetProductsByCategory` with card options and card detail. Filter options by selected restaurant, let the customer choose among active cards, and render ordered section chips and offer cards.
2. Display offer price, image, description, and base ingredients. Load offer-specific customization and only eligible priced extras; hide retired/unavailable offers as dictated by server responses. Card options require `MenuCard.View`; verify Guest and Customer grants and anonymous behavior end to end.
3. Handle no active cards, empty sections, load errors, stale selections, and a card belonging to another restaurant. A stale card or restaurant must not cause a fallback to another restaurant.

**Dependencies:** backend Phase 5, restaurant-filtered active card options and detail, card/section/listing management APIs, and offer customization contract. Card detail DTO in the analysis is a proposal; verify actual fields and ordering.

**Exit:** customer menu is restaurant- and card-scoped; section order and offer placement match management state; catalog categories no longer drive public menu navigation.

## 5. Shopping Cart module

1. Refactor `Features/ShoppingCart/Baskets/ProductCustomization` so a slice root orchestrates offer loading and add-to-basket calls; dialog components receive UI models and callbacks only. Replace product-ID-based selection with restaurant ID, offer ID, selected extra ingredient IDs, and quantity. Display base ingredients as included; offer only eligible extras and their restaurant prices. Preserve comments only if supported by the final contract.
2. Display the current unit price as offer price plus selected extra prices, and the line total as unit price times quantity. Treat displayed prices as estimates: the server resolves prices and availability on add and at checkout. Do not multiply a server-provided line total by quantity again.
3. Update `ShoppingCartApiClient`, basket summary/menu, checkout models and views to carry the basket's restaurant ID on every basket, shipping, payment, checkout, and order request as required by the final contract. Show restaurant identity in basket and checkout; preserve existing shipping and payment workflows.
4. On an attempted add from another restaurant, show a localized confirmation identifying both restaurants and explaining that the old basket will be replaced. Decline or dismiss leaves the old basket untouched. On confirmation, use a server-supported replacement operation; handle concurrent changes and failures without falsely reporting success or silently losing the existing basket. Establish the atomic replacement contract before implementing this flow.
5. Consume server revalidation results before checkout and at final submission. Show changed-price badges on affected product/basket cards and a checkout notice with old and current prices; charge the current price without an extra confirmation. Explain unavailable offers/extras and block checkout until removed or replaced. Refresh totals from authoritative server data, not snapshots.
6. Handle changes between preview and final submission: display the server's latest price or unavailability outcome and keep the user informed without creating a duplicate order. Order confirmation/history, where present, must display saved restaurant, currency, quantity, and final charged line prices rather than rebuilding from the current menu.
7. Move API calls out of `ProductCustomizationDialog` and checkout child components into their owning workflow roots or an appropriate orchestration service; refactor existing `ShoppingCartApiClient` into resource-scoped clients as the slice boundaries require. Localize dialogs, checkout steps, errors, and empty states.

**Dependencies:** backend Phases 6-7: offer-based basket contract, restaurant context propagation, atomic basket replacement, structured revalidation/price-change responses, final checkout/order contract, and immutable order purchase facts. The current checkout UI is not evidence that final order submission already exists.

**Exit:** one restaurant per basket; declining replacement preserves it; unavailable items block checkout; current prices and final order facts are visible and authoritative.

## Delivery order and verification

1. Agree final backend contracts and guest/employee context handling; implement section 1 and shared architecture first.
2. Deliver catalog slices after backend Phase 2, then offer and extra-price slices after Phases 3-4, then menu cards after Phase 5.
3. Deliver basket and checkout against Phases 6-7. Integrate navigation, localization, and responsive states incrementally rather than waiting for the end.
4. Extend architecture tests for refactored features. Add focused UI/component tests where project tooling supports them, and API integration coverage for tenant and permission enforcement. Cover authorized/unauthorized staff, guest restaurant selection, direct links and reloads, missing context, empty/error states, and two-restaurant isolation.
5. Cover catalog price removal and retirement, offer price boundaries, free versus unpriced extras, ordered cards and duplicate listings, restaurant switch accept/decline/failure, quantity totals, changed prices, unavailable items, final-submit revalidation, and historical order prices. Verify desktop and phone layouts, localized labels/errors, and keyboard/touch operation.
