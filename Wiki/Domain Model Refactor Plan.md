# Domain Model Refactor Plan: Catalog, Snapshots, and Restaurants

## Goal

Separate Company-scoped product identity from restaurant-specific offers, reduce mutable product duplication in baskets, and preserve complete immutable purchase facts in orders. Decide whether effective prices are Company-wide or restaurant-specific before assigning price ownership. Keep module boundaries intact: Shopping Cart and Orders obtain Menu data through the existing external service/contracts rather than referencing Menu domain entities directly.

## Phase 1: Confirm contracts and inventory

1. Inspect Menu, Shopping Cart, and Orders aggregates, persistence configurations, migrations, API/service contracts, and integration tests. Record current basket/order restaurant ownership and the exact IDs and fields persisted for products and ingredients.
2. Decide the open questions in `Domain Model Analysis.md`, especially price scope, scheduled-change visibility/effective dates, company-scoped product permissions, price-change checkout behavior, modifier ownership, and order tax/discount retention.
3. Define explicit DTO contracts for:
   - Menu product/catalog detail.
   - Restaurant menu entry and effective availability, with price located according to the approved Company-wide or restaurant-specific pricing rule.
   - Basket display projection and checkout validation result.
   - Order line snapshot.
4. Reconcile the Shopping Cart ADR status and replace any conflicting synchronization statements with one precise rule.

**Exit criteria:** Contract distinguishes Company-scoped Product ID from Restaurant Menu Entry ID; price ownership and effective-date behavior are explicit; basket price is non-authoritative; order snapshot is self-sufficient for historical display and totals.

## Phase 2: Model restaurant menus in Menu

1. Add or adapt a Company-scoped catalog and restaurant-scoped Menu Entry with Company ID, Restaurant ID, Product ID, availability, and only approved overrides. Put effective price at the approved scope; do not assume restaurant-specific pricing.
2. Add uniqueness and validation rules for restaurant/product association, price, and availability windows as applicable.
3. Update Menu queries/services so product discovery and pricing are always resolved for a restaurant; reject a request without valid restaurant context.
4. Ensure one Company catalog product can be offered by multiple restaurants, with each offer resolving effective price and availability according to the approved pricing rule.
5. Add the approved future-dated change behavior for product/offer edits and retirement, including an audit trail and clear effective-time semantics.
6. Add persistence configuration, indexes, migrations, and focused domain/persistence tests following repository patterns.

**Exit criteria:** Menu can return an offer for a valid restaurant and Company context without exposing Menu persistence/domain types to other modules. Scheduled changes take effect at the approved time and scope.

## Phase 3: Slim and scope basket state

1. Add Restaurant ID to the basket root (if absent) and enforce that every line in a basket resolves to that restaurant's menu.
2. Replace copied mutable product fields on basket lines with stable menu-entry/product IDs, selected option IDs, and quantities.
3. Retain only the minimal preview data needed by existing UX. Mark it as refreshable projection data and never use it as checkout pricing authority.
4. Update add/update/read flows to resolve Menu data through its external contract. At checkout, revalidate availability, options, and effective prices.
5. Define an explicit response when price or availability changes; do not silently submit a basket against stale data.
6. Add a data migration/backfill that maps existing copied products to product/menu-entry IDs. Preserve unresolved records for operator repair or apply an explicit, reviewed legacy fallback; do not silently discard basket lines.

**Exit criteria:** Basket is restaurant-scoped; stale copied data cannot determine accepted checkout totals; existing basket data has a safe migration path.

## Phase 4: Freeze order purchase snapshots

1. Ensure each order and order line retain Restaurant ID, source identifiers, customer-visible product/option snapshots, quantity, accepted unit/line prices, and currency.
2. Persist tax, discounts, and fees at the granularity needed to reproduce/explain the accepted order total.
3. At order creation, build snapshots from validated checkout data and verify totals before persistence.
4. Keep order reads independent of current Menu lookups; use source IDs for traceability only.
5. Add migration for any missing snapshot fields. For historical rows where facts cannot be recovered, preserve existing stored values and document unavoidable gaps rather than inventing current catalog data.

**Exit criteria:** Renaming, repricing, or removing a Menu product does not alter existing order display or totals.

## Phase 5: Authorization, API, and client updates

1. Propagate restaurant context through API requests, service contracts, basket operations, checkout, and order creation.
2. Validate that authenticated callers can access the selected Company or restaurant. Enforce tenant/restaurant filters in every read and write path, not only in UI selection.
3. Require company-scoped permissions for shared catalog changes. Define separately who can create or change restaurant offers; do not assume restaurant staff can change prices.
4. Update DTOs and Blazor views to display the effective price and scheduled-change information, and handle repricing/unavailable checkout responses.
5. Keep cross-module interactions behind contracts/integration boundaries; update architecture tests if contract placement or references change.

**Exit criteria:** A caller cannot read or mutate another restaurant's offer/basket/order by changing an identifier.

## Phase 6: Verification and rollout

- Unit tests: shared product across restaurants; approved price-scope behavior; scheduled price/availability change before and after effective time; product offered by one restaurant only; unavailable entry; invalid option; basket price change; immutable order snapshot after catalog mutation.
- Integration tests: restaurant-scoped Menu query and write; basket add/read/checkout revalidation; order creation from the validated basket; cross-restaurant access denial.
- Migration tests: existing basket/order records remain readable and have deterministic backfill behavior.
- Run architecture tests to confirm modules still communicate through approved contracts.
- Deploy schema before dependent API behavior; use a compatibility window if old and new clients may overlap. Remove legacy snapshot fields only after all consumers and migrations are verified.

## Recommended rollout order

1. Finalize contract and decisions.
2. Introduce restaurant menu entries and read APIs.
3. Deploy additive schema and backfill IDs.
4. Switch basket writes/checkout to restaurant-scoped references and revalidation.
5. Ensure order snapshot completeness.
6. Remove obsolete basket copies after compatibility and migration checks.

Do not combine database cleanup with the first behavioral deployment. Additive changes and a compatibility window reduce rollback risk.
