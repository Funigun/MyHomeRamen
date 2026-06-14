# Checkout Design Plan

## 1. Overview

The `/checkout` page implements a 4-step MudBlazor stepper flow.  
`CheckoutPage.razor` owns all state via `CheckoutModel` and coordinates navigation between steps.  
Steps are dumb components: they receive data/callbacks, emit results upward — no direct API calls.  
All API calls live in module-scoped `ApiClient` services.

---

## 2. CheckoutModel — shared state carrier

`CheckoutModel` is the single source of truth threaded through all steps via `[Parameter]`.  
Extend it to carry the full checkout session state:

```
CheckoutModel
├── BasketId                      — populated after Step 1
├── BasketItems                   — list of BasketItemTableModel
├── SelectedShippingOptionId      — populated after Step 2
├── ShippingAddress               — address fields (street, city, zip, notes)
├── SelectedPaymentMethodId       — populated after Step 3
├── OrderId                       — populated after Step 4 (CreateOrder)
│
├── ShoppingCartConfirmed (bool)
├── ShippingAddressConfirmed (bool)
├── PaymentMethodConfirmed (bool)
└── OrderConfirmed (bool)
```

---

## 3. CheckoutPage — orchestrator

`CheckoutPage.razor` is the only routable component (`@page "/checkout"`).  
Responsibilities:
- Instantiate and hold `CheckoutModel`
- Wire `MudStepper` callbacks: `OnPreviewInteraction` → `ControlStepCompletion` / `ControlStepNavigation`
- Block forward navigation until current step's `Confirmed` flag is set
- Pass `CheckoutModel` as `[Parameter]` to each step
- Pass `EventCallback<CheckoutModel> OnStepCompleted` to each step so steps write back to the model

Navigation guard logic (in `ControlStepCompletion`):
```
Step 0 (BasketDetails)   → allow next only if ShoppingCartConfirmed
Step 1 (ShippingDetails) → allow next only if ShippingAddressConfirmed
Step 2 (PaymentDetails)  → allow next only if PaymentMethodConfirmed
Step 3 (Summary)         → place order on confirm, set OrderConfirmed
```

---

## 4. Step Components

### Step 1 — BasketDetailsStep

**Purpose:** Review basket, edit quantities, delete items.  
**Data in:** `CheckoutModel` (BasketItems pre-populated by page `OnInitializedAsync`)  
**API features used:**
- `ShoppingCart.GetCurrentBasketDetails` — load on init (called from page, result passed via model)
- `ShoppingCart.DeleteBasketItem` — per-row delete action
- `ShoppingCart.EditBasketItem` (or existing edit/update feature) — quantity/comment edit

**On complete:**
- Sets `CheckoutModel.ShoppingCartConfirmed = true`
- Invokes `OnStepCompleted`

**Sub-components:**
- `BasketItemsTable.razor` — already exists, extend to support edit/delete actions bound to step callbacks

---

### Step 2 — ShippingDetailsStep

**Purpose:** Choose shipping option + fill delivery address.  
**Data in:** `CheckoutModel`  
**API features used:**
- `Orders.GetShippingOptions` — new feature, loads available shipping options (e.g. delivery, pickup)
- `ShoppingCart.UpdateShippingDetails` — new feature, persists chosen option + address onto the basket

**On complete:**
- Sets `CheckoutModel.SelectedShippingOptionId`, `CheckoutModel.ShippingAddress`
- Sets `CheckoutModel.ShippingAddressConfirmed = true`
- Invokes `OnStepCompleted`

**New HTTP clients needed:**
- `OrdersApiClient` — new, for `Orders.GetShippingOptions`
- `ShoppingCartApiClient` — extend with `UpdateShippingDetailsAsync`

**Sub-components:**
- `ShippingOptionsSelector.razor` — radio list of shipping options
- `ShippingAddressForm.razor` + `ShippingAddressModel.cs` + `ShippingAddressValidator.cs`

---

### Step 3 — PaymentDetailsStep

**Purpose:** Select payment method and channel.  
**Data in:** `CheckoutModel`  
**API features used:**
- `Payments.GetAvailableMethods` — existing feature

**On complete:**
- Sets `CheckoutModel.SelectedPaymentMethodId`
- Sets `CheckoutModel.PaymentMethodConfirmed = true`
- Invokes `OnStepCompleted`

**New HTTP clients needed:**
- `PaymentsApiClient` — new, for `Payments.GetAvailableMethods`

**Sub-components:**
- `PaymentMethodSelector.razor` — card/radio list rendering method + channels

---

### Step 4 — CheckoutSummaryStep

**Purpose:** Read-only review of all selections before placing order.  
**Data in:** `CheckoutModel` (all confirmed data)  
**API features used:**
- `Orders.CreateOrder` — new feature, called on final confirm button click

**On complete:**
- Sets `CheckoutModel.OrderId` from response
- Sets `CheckoutModel.OrderConfirmed = true`
- Invokes `OnStepCompleted` → page redirects to order confirmation

**Sub-components:**
- `BasketSummaryMenu.razor` — already exists, reuse for items display
- Order total / shipping / payment summary rendered inline

---

## 5. API Client Additions

| Client | Module | New methods |
|--------|--------|-------------|
| `ShoppingCartApiClient` | ShoppingCart | `GetCurrentBasketDetailsAsync`, `UpdateShippingDetailsAsync` |
| `PaymentsApiClient` | Payments | `GetAvailableMethodsAsync` — **new client** |
| `OrdersApiClient` | Orders | `GetShippingOptionsAsync`, `CreateOrderAsync` — **new client** |

`PaymentsApiClient` and `OrdersApiClient` must be registered in `Presentation/ApiDependencyInjection.cs`.

---

## 6. New Backend Features Required

| Module | Feature | Notes |
|--------|---------|-------|
| `ShoppingCart` | `UpdateShippingDetails` | Persists `ShippingOptionId` + address fields on basket |
| `Orders` | `GetShippingOptions` | Returns available shipping option types |
| `Orders` | `CreateOrder` | Creates order from confirmed basket; returns `OrderId` |

---

## 7. Data Flow Diagram

```
CheckoutPage.razor
│   OnInitializedAsync → ShoppingCartApiClient.GetCurrentBasketDetailsAsync
│   Holds: CheckoutModel
│   Guards: MudStepper OnPreviewInteraction
│
├── [Step 1] BasketDetailsStep
│     receives: CheckoutModel
│     calls:    ShoppingCartApiClient.DeleteBasketItem
│               ShoppingCartApiClient.EditBasketItem
│     emits:    OnStepCompleted(CheckoutModel { ShoppingCartConfirmed = true })
│
├── [Step 2] ShippingDetailsStep
│     receives: CheckoutModel
│     calls:    OrdersApiClient.GetShippingOptionsAsync
│               ShoppingCartApiClient.UpdateShippingDetailsAsync
│     emits:    OnStepCompleted(CheckoutModel { ShippingAddressConfirmed = true })
│
├── [Step 3] PaymentDetailsStep
│     receives: CheckoutModel
│     calls:    PaymentsApiClient.GetAvailableMethodsAsync
│     emits:    OnStepCompleted(CheckoutModel { PaymentMethodConfirmed = true })
│
└── [Step 4] CheckoutSummaryStep
      receives: CheckoutModel (read-only review)
      calls:    OrdersApiClient.CreateOrderAsync
      emits:    OnStepCompleted(CheckoutModel { OrderConfirmed = true, OrderId = ... })
                → Page navigates to /orders/{OrderId}/confirmation
```

---

## 8. Component & File Map

```
Features/ShoppingCart/
├── Baskets/
│   ├── CheckoutPage.razor                   — orchestrator, stepper, nav guards
│   ├── CheckoutPage.razor.cs                — code-behind (complex lifecycle + model init)
│   ├── Models/
│   │   ├── CheckoutModel.cs                 — EXTEND with full state fields
│   │   └── ShippingAddressModel.cs          — NEW: address form model + ToRequest()
│   └── Components/
│       ├── BasketDetailsStep.razor          — IMPLEMENT (uses BasketItemsTable)
│       ├── ShippingDetailsStep.razor        — IMPLEMENT (uses new sub-components)
│       │   ├── ShippingOptionsSelector.razor — NEW
│       │   └── ShippingAddressForm.razor    — NEW
│       ├── PaymentDetailsStep.razor         — IMPLEMENT (uses PaymentMethodSelector)
│       │   └── PaymentMethodSelector.razor  — NEW
│       └── CheckoutSummaryStep.razor        — IMPLEMENT (uses BasketSummaryMenu)
└── Common/
    └── Services/
        └── ShoppingCartApiClient.cs         — EXTEND with new methods

Features/Payments/
└── Common/
    └── Services/
        └── PaymentsApiClient.cs             — NEW

Features/Orders/
└── Common/
    └── Services/
        └── OrdersApiClient.cs               — NEW
```

---

## 9. Open Questions / Decisions for Review

1. **Guest checkout:** Does the flow allow unauthenticated (guest) users? `BasketStatus` has no auth enforcement — decision needed before implementing `UpdateShippingDetails` + `CreateOrder`.
2. **UpdateShippingDetails scope:** Should shipping details be stored on the `Basket` domain entity (extending it), or as a separate `ShippingDetails` value object? Domain change impacts migration.
3. **CreateOrder contract:** What basket fields does `CreateOrder` consume — basket ID only, or does it embed items + shipping + payment selection?
4. **OrdersApiClient auth handler:** `CreateOrder` is likely customer-scoped (`AuthHeaderHandler`), but `GetShippingOptions` may be anonymous — needs clarification before client registration.
5. **Step back behaviour:** If user goes back from Step 2 → Step 1 and modifies basket, should `ShippingAddressConfirmed` be reset? Needs guard logic defined.
