# Frontend Plan: Shopping Cart - Phase 5 Basket and Checkout

## 1. Problem

Shopping Cart must use restaurant offers and restaurant-specific extras, preserve one restaurant per basket, and present server-authoritative prices through checkout and immediate confirmation. Do not wire final submission until basket replacement, checkout idempotency/recovery, revalidation, and final charged-order facts are available from the backend.

## 2. File tree

```text
MyHomeRamen.Blazor/MyHomeRamen.Blazor/
├── appsettings.json [modify]
└── Features/
    ├── Menu/RestaurantMenu/RestaurantMenuView.razor [modify]
    └── ShoppingCart/
        ├── DependencyInjection.cs [modify]
        ├── Baskets/
        │   ├── BasketResources.cs [modify]
        │   ├── BasketResources.resx [modify]
        │   ├── BasketMenu/
        │   │   ├── BasketItemsTable.razor [modify]
        │   │   ├── BasketMenuPanel.razor [modify]
        │   │   └── BasketSummaryMenu.razor [modify]
        │   ├── CheckoutPage.razor [modify]
        │   ├── CheckoutView.razor [modify]
        │   ├── Checkout/
        │   │   ├── CheckoutModel.cs [modify]
        │   │   ├── BasketDetails/
        │   │   │   ├── BasketDetailsStep.razor [modify]
        │   │   │   ├── BasketItemsTable.razor [modify]
        │   │   │   ├── CheckoutBasketItemModel.cs [modify]
        │   │   │   ├── CheckoutIngredientModel.cs [modify]
        │   │   │   └── CheckoutProductModel.cs [modify]
        │   │   ├── PaymentDetails/
        │   │   │   ├── PaymentDetailsModel.cs [modify]
        │   │   │   └── PaymentDetailsStep.razor [modify]
        │   │   ├── Shared/CheckoutApiContracts.cs [create]
        │   │   ├── Shared/CheckoutApiClient.cs [create]
        │   │   ├── ShippingDetails/
        │   │   │   ├── ShippingAddressModel.cs [modify]
        │   │   │   ├── ShippingAddressValidator.cs [modify]
        │   │   │   ├── ShippingDetailsModel.cs [modify]
        │   │   │   └── ShippingDetailsStep.razor [modify]
        │   │   └── Summary/CheckoutSummaryStep.razor [modify]
        │   ├── ProductCustomization/
        │   │   ├── IngredientCustomizationModel.cs [modify]
        │   │   ├── ProductCustomizationDialog.razor [modify]
        │   │   ├── ProductCustomizationModel.cs [modify]
        │   │   └── ProductCustomizationValidator.cs [modify]
        │   ├── Shared/
        │   │   ├── BasketRoutes.cs [modify]
        │   │   └── BasketsApiClient.cs [create]
        │   └── Models/BasketViewModel.cs [modify]
        └── Common/Services/
            ├── ShoppingCartApiClient.cs [delete]
            └── Contracts/Baskets/
                ├── DTOs/
                │   ├── BasketDetailsIngredientDto.cs [modify]
                │   ├── BasketDetailsItemDto.cs [modify]
                │   ├── BasketDetailsItemProductDto.cs [modify]
                │   ├── BasketIngredientDto.cs [modify]
                │   ├── BasketSummaryItemDto.cs [modify]
                │   └── ShippingAddressDto.cs [modify]
                ├── Requests/
                │   ├── AddItemToBasketRequest.cs [modify]
                │   ├── UpdatePaymentDetailsRequest.cs [modify]
                │   └── UpdateShippingDetailsRequest.cs [modify]
                └── Responses/
                    ├── AddItemToBasketResponse.cs [modify]
                    ├── GetCurrentBasketDetailsResponse.cs [modify]
                    ├── GetCurrentBasketSummaryResponse.cs [modify]
                    ├── PaymentDetailsResponse.cs [modify]
                    └── ShippingDetailsResponse.cs [modify]
MyHomeRamen.ArchitectureTests/ConventionTests/BlazorSliceConventionsTests.cs [modify]
```

## 3. Basket and offer customization

### 3.1 API contracts
- Replace product-ID selection with restaurant ID, offer ID, selected extra-ingredient IDs, and quantity; keep comments only if supported by final contract.
- Carry basket restaurant ID on every basket, shipping, payment, checkout, and order request requiring it; attach `X-Restaurant-Id` per request and never mutate shared `HttpClient` headers.
- Keep basket-ID operations bound to the loaded basket restaurant. API remains authoritative for user, restaurant, offer, extra, price, and availability checks.
- Add contracts for atomic conditional basket replacement, preview/final revalidation, idempotent checkout, recoverable failures, and final charged-order summary only after backend guarantees are implemented.

### 3.2 API client
- Replace module-wide `ShoppingCartApiClient` with resource-scoped `BasketsApiClient` and `CheckoutApiClient`; register both as typed clients in ShoppingCart `DependencyInjection.cs`.
- Move basket reads/writes and restaurant-context headers into `BasketsApiClient`; put checkout revalidation/submission and idempotency key handling in `CheckoutApiClient`.
- Every method returns `ApiResult<T>` through `ApiService.ExecuteAsync`; non-success responses use shared API error handling.
- Keep payment-method operations on the existing Payments client; do not duplicate Payment API transport in ShoppingCart.

### 3.3 UI models
- Map offer/basket responses to UI models with offer price, selected extras, restaurant, currency, and authoritative line totals.
- Show base ingredients as included; eligible custom extras carry restaurant prices. Compute estimates from offer plus extras, quantity-multiplied once; replace estimates with server totals on add/revalidation.
- Preserve old/current prices and availability outcome per affected line. Include final order ID, saved restaurant, currency, quantities, charged line prices, and totals in immediate confirmation model.
- Keep checkout form models and validators aligned to existing shipping/payment requirements; no customer-selectable payment currency.

### 3.4 Components
- Make `ProductCustomizationDialog` presentation-only: UI model, validation/errors, and callbacks; remove API injection and HTTP exception handling.
- Show included base ingredients and selectable eligible extras, quantity, estimate, and localized comments only where contract supports them.
- Update basket, shipping, payment, and summary components to receive view models and callbacks; child components do not call API clients.
- Add clear restaurant-switch confirmation identifying old and requested restaurants. Decline/dismiss leaves current basket unchanged; confirm invokes conditional server replacement.
- Show changed-price badges and current-price notice; mark unavailable offers/extras and block checkout until removed/replaced.
- Design stacked forms, touch-friendly actions, and narrow-screen basket cards using the same workflow.

### 3.5 View
- Keep `CheckoutView` as workflow owner for basket load, shipping/payment save, pre-submit revalidation, final submission, separate load/submit errors, and confirmation state.
- Keep offer loading and add-to-basket orchestration in the existing restaurant-menu workflow root; it opens the presentation-only customization dialog, invokes `BasketsApiClient`, and handles replacement confirmation. Do not move API operations into the dialog.
- On replacement conflict/failure, retain and reload authoritative current basket; never report success or silently discard it.
- Before checkout, consume server revalidation and show changed/unavailable lines. At final submission, display returned authoritative charged facts and do not rebuild confirmation from current menu data.
- Store one stable idempotency key across retries for one logical submission; handle replays and conflicts using server result rather than creating duplicate UI success state.
- Maintain distinct load and submit `ApiErrorState`; preserve current form/content after failures and reset submitting flags in `finally`.

### 3.6 Page
- Preserve current checkout route and authorization/layout behavior. Keep `CheckoutPage` a thin route/title wrapper rendering `CheckoutView`.
- Keep basket restaurant identity from the saved basket, not the current browsing restaurant context.
- Do not add order history or order-management routes; immediate checkout confirmation remains in scope.

### 3.7 Resources
- Extend Basket resources for restaurant identity, included/extra ingredients, estimated/current prices, replacement prompt, stale/unavailable items, revalidation, checkout errors, and final confirmation.
- Localize all application-owned checkout and basket text, including unknown-error fallbacks; map stable API error codes/validation keys without parsing server messages.

## 4. Decisions needed
- Do not wire final checkout until backend defines and tests stable idempotency/replay, conditional replacement and checkout concurrency, durable partial-failure recovery, structured revalidation, and authoritative final order facts.
- Confirm API endpoint/payload/error-code contracts, comment support, and authoritative payment/target currency before client implementation. Display-only `Currency` configuration is established in Menu Offers phase.
- Order management/history, taxes, discounts, fees, refunds, and customer order history remain out of scope.
