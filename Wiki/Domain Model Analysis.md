# Domain Model Analysis: Catalog, Restaurant Offers, Baskets, and Orders

## Purpose

This is a discussion draft, not an approved domain-model change. It explains the current model, a proposed direction, and decisions needed before implementation.

## The short version

Keep Menu as the source of product identity, but separate a product from a restaurant's offer of that product. Here, "global product" means shared across restaurants in one Company, not shared across every Company on the platform. A restaurant offer determines whether that restaurant sells the product and may determine its effective price.

Keep basket data temporary and re-check it against Menu at checkout. Keep order data as a permanent snapshot of what the customer bought and paid for.

Example: the same ramen exists once in a Company's catalog and is offered by Restaurant A and Restaurant B. If price is restaurant-specific, each offer can have its own price; if the business requires one Company-wide price, both offers follow the same price rule. A basket points to one restaurant's offer. Once checked out, the order keeps the accepted name, customizations, and price even if the catalog changes later.

## What exists today

| Area | Current model and evidence | Gap this analysis identifies |
|---|---|---|
| Menu | `Menu.Product` owns name, description, price, image, ingredients, and categories (`MyHomeRamen.Domain/Menu/Products/Product.cs`). | No restaurant offer entity or restaurant-specific price/availability is defined in the reviewed Menu ADR or product model. |
| Basket | `ShoppingCart.Basket` has a user and copied product records, but no restaurant ID (`MyHomeRamen.Domain/ShoppingCart/Baskets/Basket.cs`). The copied product includes `OriginalId`, name, description, price, image, and ingredients (`MyHomeRamen.Domain/ShoppingCart/Products/Product.cs`). | The basket copy can become stale, and the model does not say which restaurant owns it or which price is authoritative. |
| Order | `Orders.Order` stores product records. Each copied product has an `OriginalId`, name, original/calculated prices, paid state, and ingredients (`MyHomeRamen.Domain/Orders/Orders/Order.cs`, `MyHomeRamen.Domain/Orders/Products/Product.cs`). | The reviewed order model does not show a restaurant ID, currency, or explicit tax/discount/fee breakdown. Confirm required historical and accounting fields before changing it. |
| Module contracts | The ADRs expect modules to communicate through external services rather than depend on each other's domain models. | New Menu data must be exposed as contracts/DTOs; Shopping Cart and Orders should not reference Menu entities directly. |

These are observations about the reviewed files and ADRs, not proof that no restaurant context exists elsewhere in the application. Inventory API, persistence, and client flows before implementation.

## Proposed model and behavior

| Concept | Proposed responsibility | What changes |
|---|---|---|
| Company Catalog Product | Shared product identity and information across that Company's restaurants. | Keep identity and genuinely shared fields in Menu, scoped to Company. Do not expose one Company's catalog to another. |
| Restaurant Menu Entry (new or equivalent) | Connect one product to one restaurant; own availability and any restaurant-specific overrides. Own effective price if prices may differ by restaurant. | Add a restaurant-scoped offer and query products through restaurant context. If price must be identical across the Company, define that as an explicit rule instead of accidental duplicated values. |
| Basket | Temporary selection for exactly one restaurant; retain offer/product references, selected option references, and quantities. | Add/require restaurant context. Remove copied data that is not needed for display; any retained preview is refreshable and not checkout authority. |
| Order line | Permanent record of what was accepted and charged. | Capture purchase facts at checkout. Historical display and totals must not depend on later Menu lookups. |

“Restaurant-exclusive product” does not need a separate product identity: it can be a Company catalog product offered by only one restaurant. Restaurant-specific name, image, availability, or other presentation belongs on the offer only if the business confirms those fields can vary by restaurant.

### Company-scoped product management

The accepted authorization design makes Company the tenant boundary and defines CompanyOwner as company-scoped (`Wiki/Users ADR/0004-company-restaurant-roles-and-authorization.md`). Shared catalog changes should therefore require a company-scoped permission, not a restaurant-scoped permission. Restaurant staff should not be able to change shared product data merely because they manage one restaurant.

The ADR's initial permission catalog does not list Menu product-management permissions. Define the needed company-scoped permissions (for example, view/create/update/retire catalog products) and decide whether CompanyOwner gets them by default or assigns them to another company role. The current `Role` entity only has nullable `RestaurantId`; check that implementation against the accepted explicit Company/Restaurant role-scope design before building authorization around it.

### Planned edits and retirement

Do not treat every edit as an immediate overwrite. A company-authorized user should be able to choose what changes, where it applies, and when it takes effect:

1. Edit shared product facts for the Company's catalog, or edit an individual restaurant offer for restaurant-specific facts.
2. Preview the affected restaurants and current versus proposed values.
3. Set an effective date/time. Until then, current values remain active; at that time, the scheduled values become active.
4. Show the planned change to the intended audience before it takes effect, such as company/restaurant operators and, if required, customers.
5. Keep an audit trail of who scheduled, changed, canceled, or rescheduled it.

Treat delete as retirement, not physical deletion. Retire a product across the Company only when it should leave every restaurant; otherwise disable or end-date the individual restaurant offer. Existing orders must remain readable. At checkout, validate against the offer effective at that time and handle newly unavailable products explicitly.

The model may need scheduled/effective-dated changes for product facts and restaurant offers. Whether that means versioned records or a simpler scheduled-change record is an implementation choice after confirming audit, rollback, and query needs.

### Basket lifecycle

1. When adding an item, resolve the restaurant's current offer through the Menu contract.
2. Store stable IDs, selected options, and quantity. Keep only the display preview needed by the UI.
3. Before checkout, re-check that the offer and selected options are available and obtain current prices from Menu.
4. If price or availability changed, return a result that lets the customer respond. Do not silently charge a stale basket price.

The exact customer experience for changed prices is still a decision; explicit confirmation is the recommendation, not an existing approved policy.

### Order lifecycle

When checkout is accepted, store enough information to read and explain the order without the current catalog: source IDs for traceability, customer-visible product/option names and relevant descriptions, accepted unit and line prices, quantity, currency, and the tax/discount/fee facts required for the accepted total. Include restaurant identity and a display snapshot if needed for historical records.

After acceptance, snapshots change only through explicit correction/refund workflows. Do not rebuild old orders from current Menu data. The precise fields depend on business, accounting, and retention requirements and need confirmation.

## Decisions to confirm before implementation

| Decision | Recommended starting point | Why it matters |
|---|---|---|
| Does “global product” mean shared across all restaurants in one Company? | Yes, within Company; never across Company tenants. | Defines catalog ownership and prevents cross-tenant exposure. |
| Must the same product have the same selling price in every restaurant of a Company? | Keep effective selling price on each restaurant offer unless the business explicitly requires one Company-wide price. Company users can still publish a bulk price change to many offers. | Restaurant-level price supports local differences; a shared price is simpler only if identical pricing is a real business rule. |
| Who can create, edit, schedule, or retire shared products? | Company-scoped permission; decide which company role receives it. | Prevents restaurant-scoped users from changing shared catalog data and requires permissions not listed in the current initial catalog. |
| Which changes can be scheduled, and who sees the warning? | Schedule product/offer changes with a visible effective date; decide whether notice is for operators, customers, or both. | Changes impact different audiences; visibility and timing determine UI and notification behavior. |
| What does “delete” mean, and at what scope? | Retire the Company product only when it should disappear everywhere; otherwise end the restaurant offer. Never hard-delete referenced purchase history. | Prevents one restaurant's choice from unexpectedly removing a product for all others. |
| Can a scheduled change be canceled or rescheduled, and is approval required? | Keep an audit trail; require approval only if company operations need that workflow. | Determines lifecycle, authorization, concurrency, and operational notifications. |
| Does a basket keep its old price through a scheduled price change? | Treat basket price as a preview; use the price effective at checkout and ask for confirmation if it changed. | A saved basket should not silently lock or silently change the amount to be charged. |
| Can one basket contain products from multiple restaurants? | No: one restaurant per basket and order. | Otherwise fulfillment, pricing, delivery, and order ownership need explicit multi-restaurant rules. |
| Which product fields may vary by restaurant? | Keep shared facts on Product; put price, availability, and confirmed presentation overrides on the restaurant offer. | Determines entity shape and prevents one restaurant's edits affecting another. |
| Who owns modifiers/ingredients, and can their price or availability vary by restaurant? | Keep stable option identity in Menu; resolve effective price/availability in the restaurant context if those can vary. | Affects Menu contracts, basket references, validation, and order snapshots. |
| What must an order preserve? | At minimum, accepted names/customizations, quantity, unit/line amounts, currency, restaurant, and enough tax/discount/fee data to explain totals. | Determines immutable order fields and whether historical records meet operational/accounting needs. |
| How are existing baskets associated with a restaurant and new offer IDs? | Inventory persisted records first; define deterministic backfill and an operator-repair path for ambiguous rows. | Do not infer or discard basket data when the old model has no restaurant association. |
| What is the intended identity/lifecycle of `OriginalId` in copied products? | Confirm against API contracts, persistence, and usages before replacing it. | It may be the only link from a copied row to its Menu source. |
| Should the Shopping Cart ADR status be `Proposed` or `Accepted`? | Reconcile the front matter and body before treating its decisions as approved. | The current file says `Proposed` in metadata and `Accepted` in the body. |

These are business/domain decisions. Implementation should not silently turn the recommendations above into approved policy.

## ADR evidence and unresolved contract

- Menu ADR (`Wiki/Menu ADR/0001-menu-domain.md`) makes Menu the product source of truth and expects Shopping Cart and Orders to keep copies. It says product changes during working hours are not expected and no synchronization mechanism is required.
- Shopping Cart ADR (`Wiki/Shopping Cart ADR/0001-shopping-cart-domain.md`) says copied data must be consistent with Menu when adding items and before checkout, while also saying no synchronization mechanism is required. Specify those validation points and their failure/price-change responses in the contract.
- Orders ADR (`Wiki/Orders ADR/0001-orders-domain.md`) describes order creation from Shopping Cart and consistency with Menu, but does not define the complete immutable snapshot contract.
- The reviewed ADRs do not define restaurant ownership, restaurant-specific offers, or restaurant-specific price/availability.
- Identity ADR (`Wiki/Users ADR/0004-company-restaurant-roles-and-authorization.md`) defines Company as the tenant boundary and CompanyOwner as company-scoped. It lists an initial permission catalog but no Menu product-management permissions.

## Scope of likely implementation work

If the proposed direction is approved, expect changes across Company-scoped catalog ownership and permissions, Menu modeling and queries, scheduled product/offer changes, external Menu contracts, basket restaurant ownership and stored line data, checkout revalidation, order snapshot fields, persistence/migrations, API/client flows, authorization, and tests. The detailed sequence and rollout considerations are in `Wiki/Domain Model Refactor Plan.md`.

Do not begin destructive basket-data cleanup or remove existing snapshot fields until data inventory, migration behavior, and consumers are verified.
