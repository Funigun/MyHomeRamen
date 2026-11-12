# Menu Card Mock Implementation Plan

## Goal

Rebuild the MenuCard Edit experience around the `stitch_customer_view_top_actions_layout` mock, including its intended interactions and multi-offer assignment. Preserve the existing application shell and Blazor slice ownership.

## Scope and constraints

- Keep `EditMenuCardView` responsible for loading, authorization, API calls, errors, and refreshes.
- Keep shared components presentational: parameters and callbacks only.
- Reuse existing menu-card activation, section, and offer operations where they meet the requirement.
- Add an additive bulk-assignment API contract; keep the existing single-offer endpoint for compatibility.
- Do not change domain entities, business rules, database schema, or add a migration.
- Use available product and offer data. Product description, image, and categories already exist; offer price and status already exist.
- Do not invent SKU or inventory information: neither is currently modeled.
- HTML prototype has static data and demo-only JavaScript. Implement the intended behavior, not its script.

## Proposed bulk-assignment contract

```http
POST /api/menu/cards/{menuCardId}/sections/{sectionId}/offers/batch
```

Request:

```json
{
  "offerIds": ["<offer-id-1>", "<offer-id-2>"]
}
```

Response: `201 Created` with one result per added listing, containing `offerId`, new `listingId`, and assigned `sortOrder`.

Contract behavior:

- Require at least one ID, reject duplicate IDs, and define a maximum batch size (recommended initial cap: 100).
- Preserve submitted ID order and append offers after the section's current maximum sort order.
- Reject the entire request if any offer is unavailable, belongs to another restaurant, is already listed, or the target card/section is invalid.
- Persist all additions in one handler operation and one `SaveChangesAsync` call, so the batch is all-or-nothing.
- Keep `POST .../offers` unchanged for existing callers.

## Implementation phases

### 1. Add batch-assignment feature

Create `AddMenuSectionOffers` feature under `MyHomeRamen.Features\Menu\Features\MenuCards`:

- Define request/response records, command, authorization policy, validator, handler, and endpoint.
- Reuse existing menu-card authorization requirements.
- Validate the menu card belongs to the selected restaurant and the section belongs to that card.
- Validate request constraints and that all selected offers are eligible for that restaurant.
- Load all selected offers set-wise, preserve request order, call existing `MenuSection.AddOffer` for each, and save once.
- Generate listing IDs in the handler; compute appended sort orders from the target section.
- Add a batch loader method to `IRestaurantProductOfferLoader` and its persistence implementation if needed for tracked offer loading.

Update the Blazor API layer:

- Add matching request/response contracts and a `MenuCardsApiClient` method in `MyHomeRamen.Blazor\MyHomeRamen.Blazor\Features\Menu\MenuCards\Shared`.
- Preserve the existing single-offer client method.

### 2. Enrich MenuCard management response

The current management response exposes only offer IDs/names in the picker and limited details for listed offers. Extend the read contracts and mappings to return fields needed by the mock:

- Listed offers: description, image URL, categories, price, and availability.
- Eligible offers: description, image URL, categories, price, and status/availability where applicable.

Likely files:

- `MyHomeRamen.Features\Menu\Features\MenuCards\GetMenuCardForManage\GetMenuCardForManageEndpoint.cs`
- `MyHomeRamen.Features\Menu\Features\MenuCards\GetMenuCardForManage\GetMenuCardForManageQuery.cs`
- `MyHomeRamen.Features\Menu\Features\Offers\Common\IRestaurantProductOfferQuery.cs`
- `MyHomeRamen.Persistance\Menu\Offers\RestaurantProductOfferQuery.cs`
- `MyHomeRamen.Persistance\Menu\MenuCards\MenuCardQuery.cs`
- Blazor `MenuCardApiContracts.cs` and `MenuCardViewModel.cs`

Project fields from existing `Product` and `RestaurantProductOffer` data; no domain changes required. Add category names to the projection for picker filters. Do not display SKU or stock labels without a real source for those values.

### 3. Rebuild Edit page and components

Keep the slice workflow in `EditMenuCardView.razor`; replace or revise its presentation components:

- `EditMenuCardView.razor`: compose editorial header, visibility control, section canvas, assignment dialog, and status/error states.
- Visibility control: use existing activate/deactivate APIs and respect existing `CanEdit`/`CanDelete` authorization behavior.
- `MenuCardSectionsEditor.razor`: render responsive section cards, add/delete controls, inline rename with explicit save/cancel, and reorder affordance.
- `MenuSectionListingsEditor.razor`: render responsive offer rows/cards with image, description, category, price, remove action, and existing sort-order update behavior.
- Add a presentational assignment dialog under `Components`: search, category filters, multi-select, selection count, cancel, and confirm.
- Add a dialog callback to the slice root. Submit only newly selected offers; retain explicit per-offer removal to avoid silently removing existing assignments through an add dialog.
- On assignment failure, retain selected IDs and show the localized error. On success, reload current card data.
- For section drag reorder, update sort orders through existing section update API and reload server state on failure.
- Apply matching visual language to Create page without adding mock-specific edit behavior to Create.

### 4. Apply styling, layout, and localization

- Reuse shared `AdminLayout` and `PageHeader`; do not copy mock's static navigation, branch switcher, or header into this feature.
- Existing theme already uses the mock's core palette and Newsreader/Be Vietnam Pro typography.
- Add MenuCard-scoped surface tokens/styles for layered backgrounds, spacing, cards, and modal treatment.
- Existing drawer is dark while mock uses a light sidebar. Change shared drawer colors only if the mock shell is intended as the admin-wide direction; otherwise keep the shell and scope changes to MenuCards.
- Use MudBlazor and project CSS, not CDN Tailwind or inline JavaScript.
- Add localized text and errors to `MenuCardsResources.resx`.
- Verify keyboard access, focus handling, mobile layout, and non-hover-only actions.

### 5. Tests and verification

Add Menu API integration coverage in `MyHomeRamen.MenuApi.IntegrationTests`:

- Valid batch creates every listing and returns IDs/order.
- Submitted order is preserved and sort orders append after existing items.
- Empty, oversized, duplicate-ID, already-listed, unavailable, and unknown-offer requests fail without partial persistence.
- Invalid card/section, cross-restaurant access, and authorization failures are rejected.

Add UI tests for picker filtering, multi-selection, request payload, permission-gated visibility, retained selection on API failure, and responsive presentation where the current Blazor test setup supports it. Otherwise cover these in browser verification.

Run the relevant Menu API integration tests and solution build after implementation.

## Estimate

Approximately 4-6 developer days for API contract/handler, enriched projections, UI rebuild, localization, and verification. No domain work or database migration expected.
