# Domain Model Refactor Plan: Catalog, Offers, Menu Cards, Baskets, and Orders

## Goal

Implement `Domain Model Analysis.md`: Company-scoped catalog without prices, restaurant-level prices (offers and extra-ingredient prices), restaurant-owned menu cards, restaurant-scoped baskets, minimal order purchase facts. Backend is prepared for Company and Restaurant ownership; UI keeps single-Company behavior. Modules communicate only through external contracts (`IRestaurantService`, Menu contract), never domain entities.

Design rules:
- Line price = `OfferPrice + sum(extra ingredient prices at that restaurant)`. Offer `Price > 0`; extra `Price >= 0`.
- Missing `RestaurantIngredientPrice` = extra not offered at that restaurant.
- Changes apply immediately. No price versioning, scheduling, or audit log.
- Status is `Active`/`Retired`; offer hidden if it or its product is `Retired`; no cascade write.
- Dev database recreated: schema migrations only, no backfill.
- Out of scope: tax, discounts, fees, refunds, custom ramen builder, Company selector, Company-scoped roles.

## Blocking decisions

The developer decides these. Implementers must not pick an option.

| ID | Decision | Blocks | Status |
|---|---|---|---|
| B-1 | Permissions: `MenuCard.*`, `Offer.*`, `IngredientPrice.*` (Add/Edit/Delete/View/Manage, matching `Product.Add`); Guest and Customer get `.View`; admin = `.Manage` + one CRUD | Phase 1 | Decided |
| B-2 | `CompanyId` source: `IRestaurantService` | Phase 2 | Decided |
| B-3 | Public endpoints `menu/cards/options` and `menu/cards/{id}`, `MenuCard.View` (DTOs in Analysis) | Phase 5 | Decided |
| B-4 | Offer shared across cards, no card pricing | Phase 5 | Decided |
| B-5 | Menu sections replace `GetMenuCategories`; `Category` stays taxonomy | Phase 5 | Decided |
| B-6 | Price management by `Offer.*` permissions, restaurant-scoped grants | Phase 4 | Decided |
| B-7 | `GET restaurants/options` with address fields | Phase 1 | Decided |
| B-8 | Permission naming uses `Add`, consistent with `Product.Add` | Phase 1 | Decided |
| B-9 | Ingredient extra prices use `IngredientPrice.*` | Phase 4 | Decided |

## Phase 1: Restaurants contract and customer entry

1. Add `IRestaurantService` in `Restaurants/ExternalApi`, implementation in `Restaurants/Services`, same pattern as `IPaymentService`. Capabilities: restaurant exists and is active; Company ID of a restaurant.
2. Add anonymous-or-`Restaurant.View` `GET restaurants/options` (active only): `Id, Name, Street, City, ZipCode, Latitude, Longitude`.
3. Add `MenuCard.*`, `Offer.*`, `IngredientPrice.*` permissions (Add/Edit/Delete/View/Manage); seed `.View` for Guest and Customer roles (`IngredientPrice.View` is needed to read extra prices). Seed all of them for the owner role created in the CompanyRegistration flow, which also creates the first restaurant.
4. Reconcile Shopping Cart ADR (`Proposed` vs `Accepted` body). One rule: validate on add and at checkout.
5. Inventory Menu, Shopping Cart, Orders contracts and integration tests.

**Exit:** other modules can validate a restaurant without referencing Restaurants domain; customers can list restaurants; permissions defined.

## Phase 2: Company catalog

1. Add `CompanyId` to `Product`, `Ingredient`, `Category`, Payments; `Active`/`Retired` on `Product`. `CompanyId` comes from `IRestaurantService`.
2. Remove `Product.Price` (prices move to offers). Update dependent queries and DTOs.
3. Tenant filter on every read and write.
4. Retire (never delete) products.
5. Persistence configuration, migration, domain/persistence tests.

**Exit:** a Company's catalog is never exposed to another Company; catalog holds no prices.

## Phase 3: Restaurant offers and extra prices

1. Add `RestaurantProductOffer`: `RestaurantId`, denormalized `CompanyId`, `ProductId`, `Price`, `Active`/`Retired`, audit fields. Unique `(RestaurantId, ProductId)`.
2. Add `RestaurantIngredientPrice`: `RestaurantId`, `IngredientId`, `Price`. Unique `(RestaurantId, IngredientId)`.
3. Validate Restaurant and Product/Ingredient share a Company via `IRestaurantService`.
4. Validate `Price > 0` (offer), `Price >= 0` (extra). Hide offers where offer or product is `Retired`.
5. Only ingredients listed as the product's custom ingredients may be selected as extras, and only if priced at that restaurant.
6. Tests: shared product across restaurants with different prices, unpriced extra not offered, duplicate offer, retired product hides offer, cross-company rejection.

**Exit:** Menu returns a priced offer with available extras for a valid restaurant without exposing Menu domain types.

## Phase 4: Price management

1. Commands to create/update/retire offers and set/remove extra prices, scoped to the restaurant.
2. Authorization: `Offer.Manage` + one CRUD permission, scoped by the grant's `RestaurantId`. Extra-ingredient prices use `IngredientPrice.Manage` + one CRUD permission, same scoping.
3. Tests: manager of restaurant A cannot change prices of restaurant B.

**Exit:** a restaurant can set local prices and turn extras on or off.

## Phase 5: Menu cards and sections

1. Add `MenuCard` (`RestaurantId`, `Name`, `IsActive`), `MenuSection` (`MenuCardId`, `Name`, `SortOrder`), `MenuSectionItem` (`MenuSectionId`, offer ID, `SortOrder`). Unique `(MenuSectionId, RestaurantProductOfferId)`. Standard audit fields.
2. Keep `Category` for product taxonomy (B-5).
3. Replace `GetMenuCategories` (`api/menu/categories/menu`) with `GET menu/cards/options` (`CompanyId, RestaurantId, MenuCardId, Name`) and `GET menu/cards/{id}` (sections with offers), both `MenuCard.View`. Admin card/section commands require `MenuCard.Manage` + a CRUD permission. DTOs: see Analysis. Update `RestaurantMenuPage` to use section chips and offers.
4. Tests: multiple cards per restaurant, duplicate listing rejected, wrong-restaurant card rejected.

**Exit:** public menu is card-scoped.

## Phase 6: Basket

1. Add `RestaurantId` to `Basket`; `RestaurantProductOfferId` and selected extra ingredient IDs to basket items. Keep copied display data and price snapshot; keep `OriginalId` as reference-only `ProductId`.
2. Update add-to-basket to resolve offer and extra prices through the Menu contract and validate restaurant via `IRestaurantService`.
3. Different restaurant: ask user to confirm replacing the basket; declining keeps the current one.
4. Before checkout, revalidate offer and extras are active and priced; fetch current prices. Price change: badge item, notify on product card and checkout, charge current price. Unavailable: explain and require remove or replace.
5. Schema migration only.

**Exit:** basket is restaurant-scoped; copied data never decides totals.

## Phase 7: Orders

1. Add `RestaurantId` and `Currency` to `Order`; `Quantity`, final charged line price, offer reference to order lines. Keep `OriginalId`.
2. Checkout passes validated data through the Orders contract; orders are never rebuilt from current Menu data.
3. Schema migration only.

**Exit:** repricing or retiring a product does not change existing orders.

## Phase 8: API, minimal UI, verification

1. Propagate restaurant context through API requests, basket, checkout, and order creation. Enforce tenant and restaurant filters on every read and write.
2. Minimal Blazor: customer landing page with restaurant picker, restaurant ID carried to basket flows, effective price display, extras, price-change notices, replace-basket confirmation. Admin keeps the existing permission-filtered restaurant list and route-based restaurant context. No Company selector.
3. Tests:
   - Unit: line price calculation, retired hiding, unpriced extra rejected, basket price change, immutable order data.
   - Integration: restaurant-scoped menu, basket add/switch/checkout revalidation, order creation, cross-restaurant and cross-company denial.
   - Architecture: modules communicate only through contracts.

**Exit:** a caller cannot read or mutate another restaurant's or company's data by changing an identifier.

## Rollout order

1. Phase 1, then Phase 2.
2. Phases 3 and 4, then Phase 5.
3. Phase 6, then Phase 7.
4. Phase 8.

Development database is reset after schema changes, so no backfill. Revisit if real data appears before release.
