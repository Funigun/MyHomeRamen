# Draft Plan: Cross-Cutting API Error Contract Refactor

**Status:** Deferred until current frontend plans are complete. Do not change current API or UI behavior as part of those plans.

## 1. Problem

API and Blazor error contracts do not align. `ExceptionMiddleware` serializes `ResponseBody` with `StatusCode`, `Message`, `Errors`, and `ValidationErrors`; Blazor's `ApiRequestException` parses only 400 responses and looks for `errors`, `detail`, or `title`. Validation details can therefore be dropped and replaced with the generic localized fallback.

## 2. Goals

- Define one stable error response contract across API features.
- Give validation failures stable field identifiers and error codes, independent of English message text.
- Preserve status-based handling and localized fallback for unknown errors.
- Keep current 403 and 5xx global handling unless a later decision changes it.
- Make all Blazor API clients consume the same contract.

## 3. Draft work sequence

1. Inventory exception types, validation rules, API responses, Blazor parsing, and current user-facing error behavior across modules.
2. Decide the response format, stable error-code catalog, field-key format, localization ownership, and rollout compatibility.
3. Implement centralized backend serialization for validation, not-found, conflict, forbidden, and unexpected errors.
4. Update Blazor parsing and error state to retain status, codes, field errors, and safe fallback details.
5. Migrate feature forms and action flows to map stable field keys/codes to localized resources; preserve form state on failures.
6. Add backend mapping tests, Blazor parser tests, and focused feature tests for representative errors.

## 4. Decisions to make

- Use RFC Problem Details with extension fields or evolve existing `ResponseBody`.
- Define error codes and field keys centrally or per feature.
- Preserve compatibility with clients that consume the current response shape during rollout.
- Specify which error details may be shown to users versus logged only.

## 5. Acceptance criteria

- All API error paths produce the agreed, consistently serialized shape.
- Blazor parses the agreed shape and maps known codes/field keys to localized messages without parsing server text.
- Validation errors appear on the correct fields; business-rule failures receive actionable localized feedback.
- Unknown errors use localized fallbacks; existing global 403/5xx behavior remains covered.
- Tests cover 400 field validation, business conflict, 404, 403, 5xx, malformed/unknown payload, and compatibility requirements.
