# Plan: Restaurants - Company owner registration UI

## 1. Scope
Create one action-oriented Blazor registration slice for synchronous company owner registration. Do not create Company editing, builder, lifecycle, restaurant, or role-management UI.

## 2. UI structure
- Add `Features/Identity/RegisterCompanyOwner/` page and slice component.
- Add `Features/Restaurants/Company/Shared/CompanyNameField` for reusable company-name input and validation display.
- Add module-specific dependency-injection registration for registration and Company resource clients.
- Keep page responsible for loading state, validation, API orchestration, idempotency, error handling, and post-registration navigation.
- Keep shared field components presentation-only; they receive values and callbacks and do not call APIs or navigate.

## 3. Page and route
- Page: `RegisterCompanyOwnerPage`.
- Route: `/register/company-owner`.
- Page defines route, layout, authorization requirements, and slice composition only.
- Use a static route builder only for the returned Company destination if later navigation requires parameterized routing.

## 4. Models and API clients
- Add UI-specific `RegisterCompanyOwnerModel`; do not expose backend contracts or domain models to Razor components.
- Model includes existing customer-registration fields and a `CompanyName` field wrapper/model.
- Add mapping from UI model to registration request contract.
- Add `IdentityRegistrationApiClient` for `RegisterCompanyOwner`.
- Add `CompaniesApiClient` only for shared company resource calls needed by this slice; do not add Company editing calls.
- Keep HTTP, response parsing, and idempotency header handling inside API clients/shared HTTP infrastructure.

## 5. Validation and workflow
- Reuse existing customer registration validation rules through UI-compatible validators.
- Validate user fields before company-name availability/registration submission.
- Validate required company name and reject values that normalize to empty.
- Preserve display name in the form; backend remains authoritative for normalized uniqueness.
- Generate and preserve one `Idempotency-Key` for the submission attempt and reuse it for retries.
- Show distinct states for editing, submitting, success, duplicate company name, validation failure, unauthorized access, external registration failure, and unexpected failure.
- On `201`, retain returned `CompanyId` and navigate to the future Company builder destination only when that route exists; otherwise show success state with Company ID without inventing an edit workflow.
- Localize labels, validation messages, errors, buttons, headings, and empty/status text through resources.
- Design stacked form and touch-friendly actions for phone layouts; keep one responsive workflow.

## 6. Authorization and user experience
- Require authenticated-user context appropriate for owner registration.
- Do not make authorization decisions from translated role names.
- Surface `401`/`403` using existing authentication/authorization UX patterns.
- Treat `409` as company-name conflict and keep entered form values for correction.
- Treat retryable registration failures without losing the idempotency key or entered data.