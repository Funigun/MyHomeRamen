# Frontend Plan: Menu - Phase 3 Restaurant Offers and Ingredient Prices

## 1. Problem

Restaurant managers need to control which catalog products are offered and set restaurant-specific offer and extra-ingredient prices. Keep restaurant context explicit in routes and per-request headers so catalog data and one restaurant's prices cannot be mistaken for another's.

## 2. File tree

```text
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/
├── DependencyInjection.cs [modify]
├── MenuCards/Shared/MenuCardApiContracts.cs [modify]
├── Offers/
│   ├── OffersPage.razor [create]
│   ├── OffersResources.cs [create]
│   ├── OffersResources.resx [create]
│   ├── OffersView.razor [create]
│   ├── Components/OffersTable.razor [create]
│   ├── Components/OfferRetirementConfirmation.razor [create]
│   ├── CreateOffer/
│   │   ├── Components/CreateOfferForm.razor [create]
│   │   ├── CreateOfferPage.razor [create]
│   │   ├── CreateOfferView.razor [create]
│   │   └── Models/
│   │       ├── CreateOfferFormModel.cs [create]
│   │       └── CreateOfferFormModelValidator.cs [create]
│   ├── EditOfferPrice/
│   │   ├── Components/EditOfferPriceForm.razor [create]
│   │   ├── EditOfferPricePage.razor [create]
│   │   ├── EditOfferPriceView.razor [create]
│   │   └── Models/
│   │       ├── EditOfferPriceFormModel.cs [create]
│   │       └── EditOfferPriceFormModelValidator.cs [create]
│   ├── RetireOffer/
│   │   ├── RetireOfferPage.razor [create]
│   │   └── RetireOfferView.razor [create]
│   ├── Models/OfferViewModel.cs [create]
│   └── Shared/
│       ├── OffersApiClient.cs [modify]
│       ├── OffersApiContracts.cs [create]
│       └── OffersRoutes.cs [create]
└── RestaurantIngredientPrices/
    ├── Components/
    │   ├── IngredientPriceForm.razor [create]
    │   └── IngredientPricesTable.razor [create]
    ├── Models/
    │   ├── IngredientPriceFormModel.cs [create]
    │   ├── IngredientPriceFormModelValidator.cs [create]
    │   └── RestaurantIngredientPriceViewModel.cs [create]
    ├── RestaurantIngredientPricesPage.razor [create]
    ├── RestaurantIngredientPricesResources.cs [create]
    ├── RestaurantIngredientPricesResources.resx [create]
    ├── RestaurantIngredientPricesView.razor [create]
    └── Shared/
        ├── RestaurantIngredientPricesApiClient.cs [create]
        ├── RestaurantIngredientPricesApiContracts.cs [create]
        └── RestaurantIngredientPricesRoutes.cs [create]
MyHomeRamen.ArchitectureTests/ConventionTests/BlazorSliceConventionsTests.cs [modify]
```

## 3. Restaurant product offers

### 3.1 API contracts
- Use UI-owned offer and ingredient-price contracts, mapped at client boundaries; do not use backend/domain types.
- Carry restaurant context in route-specific actions and attach `X-Restaurant-Id` to each restaurant-scoped request. Do not mutate shared `HttpClient` headers or send a caller-supplied Company ID.
- Offer management contracts expose catalog product identity, offer status/price, and server-returned allowed actions.
- Backend adjustment required: add allowed actions to management responses, scoped to the current restaurant and computed from `Offer.Manage` plus operation permissions. Offer list response needs create permission; each offer needs edit/retire permissions, with retired offers disallowing both. Ingredient-price response needs create/edit/remove permissions. Mutations still revalidate authorization server-side.
- Price contracts distinguish no restaurant price from a valid zero price.
- Removing a restaurant price does not delete a catalog ingredient or remove its use as a base ingredient.

### 3.2 API client
- Extend the existing registered `OffersApiClient` with restaurant-scoped offer list/create/price-edit/retire calls and retain its offer-customization read.
- Add `RestaurantIngredientPricesApiClient` for list/set/edit/remove operations; register as a typed client in Menu `DependencyInjection.cs`.
- Every method returns `ApiResult<T>` through `ApiService.ExecuteAsync`; use the established response success/error helpers.
- Move offer customization transport records out of `MenuCards/Shared/MenuCardApiContracts.cs` into `Offers/Shared/OffersApiContracts.cs`; update consumers without duplicating contracts.

### 3.3 UI models
- Map catalog/offer responses to `OfferViewModel` with restaurant price, status, and allowed actions.
- Validate offer price as strictly greater than zero. Validate extra-ingredient price as zero or greater.
- Map ingredient-price list states so priced and unpriced ingredients are distinguishable; do not represent unpriced as a free extra.
- Use action-specific form models and validators where create and price-edit fields differ.

### 3.4 Components
- Add responsive offers table with explicit status and action affordances; offer price applies to every menu-card listing of that offer.
- Add create-offer and edit-price forms using display-only `Currency` from Blazor `appsettings.json`; do not treat it as API/company truth or charged currency, and do not add a currency picker.
- Add ingredient-price table/form that can set, edit, or remove a restaurant price; zero remains a valid free extra.
- Keep set/edit/remove workflows within the restaurant ingredient-price slice when fields and validation match.
- Components receive models, localized labels, validation errors, and callbacks only; no API calls, navigation, or authorization decisions.

### 3.5 View
- Offers entry root loads only the route restaurant's offers and eligible catalog products, and enforces server-returned allowed actions in addition to page authorization.
- Separate create, edit-price, and retire roots own their API operation, submission state, error handling, and post-action navigation.
- Ingredient-price management lists priced and unpriced catalog ingredients; removing a price disables an ingredient as an extra only for this restaurant.
- Never show an unpriced ingredient as a free extra.
- Explain that base ingredients are included in offer price, custom ingredients require an offer configuration and a restaurant price, and price changes apply immediately.
- Use separate load and submit `ApiErrorState` instances; preserve form state on submit failure and reset submitting state in `finally`.

### 3.6 Page
- Keep offers and ingredient-price pages beneath the target restaurant context with route constants/builders.
- Authorize pages with `Offer.Manage` or `IngredientPrice` management policy, while honoring operation-specific `Add`, `Edit`, or `Delete` permissions and server-allowed actions.
- Never treat a route restaurant ID or a visible action as proof of authorization; the API revalidates every operation.
- Do not add a Company selector or global restaurant switcher.

### 3.7 Resources
- Add slice/aggregate resources for loading, validation, empty/error states, price/status labels, retirement confirmation, and immediate-price-change explanation.
- Configure display-only `Currency` in Blazor `appsettings.json` for localized price labels; do not treat it as charged currency.
- Localize currency labels and errors; do not parse server message text or use translated display text for permission checks.

## 4. Decisions resolved from backend
- **Routes and restaurant context:** Use `GET /api/menu/offers/manage`, `POST /api/menu/offers`, `PUT /api/menu/offers/{id}`, and `DELETE /api/menu/offers/{id}` for management. `GET /api/menu/offers/{id}` remains customer customization, not management detail. Ingredient-price routes are `GET /api/menu/ingredient-prices/manage`, `POST /api/menu/ingredient-prices`, `PUT /api/menu/ingredient-prices/{ingredientId}`, and `DELETE /api/menu/ingredient-prices/{ingredientId}`. These routes use the server's current restaurant context, supplied by `X-Restaurant-Id`; do not add restaurant IDs to API paths or send Company IDs.
- **Allowed actions and page access:** Management reads require `Offer.Manage + Offer.View` or `IngredientPrice.Manage + IngredientPrice.View`; page access must honor both. Add the planned server-returned action flags, computed from the corresponding `*.Manage` permission plus `*.Add`, `*.Edit`, or `*.Delete`, and offer status; API commands continue to reauthorize. For list-level create actions use `Offer.Manage + Offer.Add` and `IngredientPrice.Manage + IngredientPrice.Add`. For offer rows use `Offer.Manage + Offer.Edit/Delete`; retired offers have edit and retire disabled. For priced ingredient rows use `IngredientPrice.Manage + IngredientPrice.Edit/Delete`.
- **Catalog choices and permissions:** Existing catalog-management reads require `Product.Manage` or `Ingredient.Manage`, which are independent of offer/price permissions. Do not silently require those extra permissions. Backend must provide offer-eligible active products for users with `Offer.Manage + Offer.Add`, and a price-management read of all company ingredients joined with the current restaurant's optional price for users with `IngredientPrice.Manage + IngredientPrice.View`. Current ingredient-price management read returns priced rows only; a missing price row means unpriced, not zero. Use price permissions for price actions, not catalog CRUD actions.
- **Errors and duplicate offers:** Duplicate-offer validation is a field validation failure (`400`) without a stable error code. Use generic field-error handling in this phase; do not introduce a feature-specific code or change the error contract. Stable error codes and the shared response/parser changes are deferred to `.github/plans/api-error-contract-refactor-draft.md`, after current frontend plans.
- **Price and offer lifecycle:** Offer price must be greater than zero; extra-ingredient price may be zero. Create rejects a product/restaurant pair if any offer already exists, including retired offers; current API has no reactivation operation. Do not offer recreate/reactivate flows.
