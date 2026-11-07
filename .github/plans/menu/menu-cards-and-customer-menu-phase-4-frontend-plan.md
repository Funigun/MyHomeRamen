# Implementation Plan: Menu - Phase 4 Restaurant Menu Cards and Customer Menu

## First implementation step: backend contract updates

Complete and test these backend changes before starting Blazor work:
- Add `SortOrder` to `MenuCard`, persistence configuration, and a migration. Assign new cards the next order after existing cards.
- Add restaurant-scoped batch card ordering, following the existing category-order endpoint pattern (`PUT /api/menu/cards/order` with card IDs and sort orders). Validate that every card belongs to the selected restaurant. Return cards in `SortOrder`, then name order, from management and customer option queries.
- Add `AllowedActionsDto` to management list and detail responses, calculated for the selected restaurant from `MenuCard.Manage` plus `MenuCard.Add`, `MenuCard.Edit`, and `MenuCard.Delete`. Keep active-state-dependent activate/deactivate choices mapped to the matching edit/delete capability; server authorization remains authoritative.
- Add focused tests for card ordering, restaurant isolation, and independent allowed-action flags. Verify Guest and Customer card reads with their permission grants.
- Keep existing section and offer reorder APIs; they already support individual `SortOrder` updates. No bulk section/offer reorder is required for this UI plan.

## 1. Problem

Restaurant managers need menu-card management, ordered cards and sections, and offer placement; customers need to browse active cards rather than catalog categories. Management and customer views must preserve restaurant scope, show server order, and keep card/listing operations separate from offer pricing.

## 2. File tree

```text
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/
├── MenuCards/
│   ├── MenuCardsPage.razor [create]
│   ├── MenuCardsResources.cs [create]
│   ├── MenuCardsResources.resx [create]
│   ├── MenuCardsView.razor [create]
│   ├── Components/
│   │   ├── MenuCardForm.razor [create]
│   │   ├── MenuCardSectionsEditor.razor [create]
│   │   ├── MenuCardTable.razor [create]
│   │   └── MenuSectionListingsEditor.razor [create]
│   ├── CreateMenuCard/
│   │   ├── CreateMenuCardPage.razor [create]
│   │   └── CreateMenuCardView.razor [create]
│   ├── EditMenuCard/
│   │   ├── EditMenuCardPage.razor [create]
│   │   └── EditMenuCardView.razor [create]
│   ├── ViewMenuCard/
│   │   ├── ViewMenuCardPage.razor [create]
│   │   └── ViewMenuCardView.razor [create]
│   ├── Models/
│   │   ├── MenuCardFormModel.cs [create]
│   │   ├── MenuCardFormModelValidator.cs [create]
│   │   └── MenuCardViewModel.cs [create]
│   └── Shared/
│       ├── MenuCardApiContracts.cs [modify]
│       ├── MenuCardsApiClient.cs [modify]
│       └── MenuCardRoutes.cs [create]
└── RestaurantMenu/
    ├── Models/RestaurantMenuViewModel.cs [modify]
    ├── RestaurantMenuPage.razor [modify]
    ├── RestaurantMenuResources.cs [modify]
    ├── RestaurantMenuResources.resx [modify]
    ├── RestaurantMenuView.razor [modify]
    └── Shared/RestaurantMenuRoutes.cs [modify]
MyHomeRamen.ArchitectureTests/ConventionTests/BlazorSliceConventionsTests.cs [modify]
```

## 3. Restaurant menu-card management

### 3.1 API contracts
- Extend UI transport contracts for card list/detail and card, section, and listing mutations; map responses into UI-specific models.
- Preserve card name and active status, card/section/listing order, restaurant identity, and allowed actions returned by the updated management APIs.
- Listings reference restaurant offers and control placement only; do not expose price editing through card workflows.
- Use `X-Restaurant-Id` for all restaurant-scoped management calls. Keep Company context server-derived.

### 3.2 API client
- Extend existing `MenuCardsApiClient` for management operations and retain customer option/detail reads; typed client is already registered in Menu `DependencyInjection.cs`.
- Wrap every operation in `ApiService.ExecuteAsync`, use the shared response helpers, and return `ApiResult<T>`.
- Keep offer customization calls on `OffersApiClient`; do not duplicate offer transport/client logic in MenuCards.

### 3.3 UI models
- Map cards, ordered sections, and ordered offer listings to UI view models while preserving server order after reload.
- Use a shared card form/model/validator only for fields intentionally common to create/edit; keep action-specific slice roots.
- Model unavailable/retired offers and returned allowed actions explicitly; do not infer permission from card status or translated labels.
- Add ordering models for reorder operations; reject a duplicate offer within one section while allowing supported placement in another section/card.

### 3.4 Components
- Add responsive card table and card form, plus section and listing editors with explicit ordering controls.
- Filter listing choices to eligible offers for the route restaurant. Show unavailable/retired offers and validation/conflict errors without dropping unsaved edits.
- Components receive data, localized labels, validation errors, and callbacks only. Keep API calls, authorization, and navigation in slice roots.

### 3.5 View
- `MenuCardsView` loads restaurant cards; create, view, and edit roots own their respective workflows. The table's remove action confirms, calls the existing deactivation endpoint, then reloads; no standalone remove page/view.
- `EditMenuCardView` orchestrates card-scoped section/listing changes, reloads authoritative order after success, and preserves local edits on failure.
- Gate operations by `MenuCard.Manage` and the relevant CRUD permission; direct routes receive the same server checks as navigation.
- Display that offer price is managed in Offers and applies across every card listing that offer.

### 3.6 Page
- Add route constants/builders in `MenuCardRoutes`; keep pages thin route/layout/authorization/title wrappers.
- Keep all management routes tied to an explicit restaurant ID. Do not add a global selected-restaurant control.

### 3.7 Resources
- Localize card/section/listing labels, reorder actions, empty/loading/error states, validation, unavailable-offer messaging, and confirmation text.
- Use stable API error codes and localized fallback for unknown errors.

## 4. Customer menu

### 4.1 API contracts
- Use existing card option/detail transport contracts, extending them only to match implemented active-card, restaurant filtering, image, offer, and ordered section/listing fields.
- Options remain headerless where the API contract documents that exception; card detail uses selected restaurant ID in `X-Restaurant-Id`.
- Card options must be filtered to the selected restaurant and active cards. A stale card/restaurant fails explicitly; never fall back to another restaurant.

### 4.2 API client
- Reuse `MenuCardsApiClient` options/detail methods and resource registration; do not create a duplicate menu client.
- Continue using `OffersApiClient` for eligible offer customization, with per-request restaurant context.

### 4.3 UI models
- Map card options, ordered sections, listings, offer image/description/price, and base ingredients into customer menu view models.
- Keep section ordering from the server; do not use catalog category IDs or names as customer menu sections.

### 4.4 Components
- Keep section chips and offer cards responsive and keyboard/touch accessible; expose explicit empty, loading, and stale-selection states.
- Offer customization remains a distinct action from card navigation and card-management presentation.

### 4.5 View
- Refactor `RestaurantMenuView` to load cards for the restaurant context from Phase 1, select active cards, and render their ordered sections/listings.
- Replace category-based menu reads with card options/detail. Show offer price, image, description, base ingredients, and only eligible priced extras returned by the offer contract.
- Handle no active cards, empty sections, load failures, and stale/mismatched card IDs without switching restaurant context.
- Preserve CompanyOwner preview behavior and route restaurant identity; do not expose customer add-to-basket actions in owner preview.

### 4.6 Page
- Preserve `/menu` and the CompanyOwner restaurant-preview route; pages stay thin and route constants remain the single navigation source.
- Require the customer browsing context established in Phase 1; when absent, return to the restaurant picker rather than guessing a restaurant.

### 4.7 Resources
- Localize card selection, section navigation, no-card/no-section/no-offer states, offer details, and all loading/error states.

## 5. Current backend baseline
- **Removal:** `DELETE /api/menu/cards/{id}` deactivates the card (`IsActive = false`); no hard-delete endpoint exists. `PUT /api/menu/cards/{id}/activate` reactivates it. Present removal as deactivation and implement it as a confirmed table action, not a separate route/page/view.
- **Management routes and permissions:** list uses `GET /api/menu/cards/manage`; detail uses `GET /api/menu/cards/{id}/manage`. Both require `MenuCard.Manage` and `MenuCard.View`. Create operations require `MenuCard.Manage` plus `MenuCard.Add`; edits/reordering require `MenuCard.Manage` plus `MenuCard.Edit`; deactivation requires `MenuCard.Manage` plus `MenuCard.Delete`. Management list/detail responses currently return no allowed-action payload. Send `X-Restaurant-Id` for management calls.
- **Ordering:** sections reorder through `PUT /api/menu/cards/{menuCardId}/sections/{sectionId}` with both `Name` and `SortOrder`; offer listings reorder individually through `PUT /api/menu/cards/{menuCardId}/sections/{sectionId}/offers/{itemId}` with `SortOrder`. No bulk section/offer reorder endpoint exists. Cards currently have no sort-order field: options sort by name, and management list supports sorting by name or active status. The first implementation step adds card ordering.
- **Customer reads:** options return card ID, restaurant ID, company ID, and name; detail returns ordered sections and active offers with price, image URL, description, and base ingredients. Options filter to active cards at active restaurants; detail only returns active cards and omits inactive offers/products. Options use `restaurantId` query parameter and are exempt from the restaurant-header requirement; card detail requires the selected restaurant in `X-Restaurant-Id`.
- **Guest/Customer access:** both roles receive `MenuCard.View`. Options and detail endpoints are marked `AllowAnonymous`, but their query authorization and per-restaurant filtering still require that permission. Guest and Customer principals with their role grants can read; a request with no persisted Guest identity/permission grants is not guaranteed access. Treat “anonymous” as the app's Guest experience, not permission-free access.
