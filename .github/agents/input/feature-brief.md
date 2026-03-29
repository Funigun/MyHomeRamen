# Feature Brief

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Menu` |
| **Feature name** | `GetCategoriesForDropdown` |
| **Short description** | Returns a lightweight list of categories filtered by `CategoryType`, ordered by `SortOrder`, for use in dropdown selectors (e.g. product creation form). |
| **Reference feature** | `CreateCategory` |
| **Source branch** | `feature/get_categories_options` |
| **Target branch** | `feature/get_categories_options` |

---

## 2) Scope

| Scope | Include? |
|---|---|
| `backend` — API + Domain + Persistence | yes |
| `frontend` — Blazor Server / WASM | yes |

---

## 3) API Details (backend scope)

| Field | Value |
|---|---|
| **HTTP method** | `GET` |
| **Route** | `/api/menu/categories/dropdown` |
| **Authorization policy** | `RestaurantManager` |
| **Applies IValidator** | yes — validate `categoryType` is a defined `CategoryType` enum value |
| **Applies ICachePolicy** | no |
| **Applies IAuthorizationPolicy** | no |

---

## 4) Domain Details (backend scope)

| Field | Value |
|---|---|
| **Aggregate / entity** | `Category` |
| **New domain entity needed** | no |
| **Domain events produced** | none |
| **Asynchronous messaging** | none |

---

## 5) Persistence Details (backend scope)

| Field | Value |
|---|---|
| **EF migration needed** | no |
| **New DbContext configuration needed** | no |
| **New DB extension method needed** | no |

---

## 6) Frontend Details (frontend scope)

| Field | Value |
|---|---|
| **Pages to create or update** | none |
| **Components to create or update** | `ProductForm.razor` — replace static `Categories` parameter with a call to the new endpoint on component init, filtering by `CategoryType.Product` |
| **API service to create or update** | `MenuApiClient` — add `GetCategoriesForDropdownAsync(int categoryType)` method |
| **Reference frontend feature** | `ProductForm.razor` (existing product form) |

---

## 7) Testing Requirements

| Test type | Required | Notes |
|---|---|---|
| Unit tests | no | — |
| Integration tests | yes | Reference `CreateCategoryTests` — cover: returns ordered list for valid type, returns empty list when no match, returns 400 for invalid `categoryType` value, requires authentication |
| Architecture tests | no | — |
| System tests | no | — |

---

## 8) Additional Notes

- `CategoryType` enum: `Product = 1`, `Ingredient = 2` — both defined in `MyHomeRamen.Domain.Menu.Categories` and mirrored in Blazor at `MyHomeRamen.Blazor.Features.Menu.Categories`.
- Results must be ordered ascending by `SortOrder`.
- Response shape must align with existing `CategoryOption(Guid Id, string Name)` record used in `ProductForm.razor`.
- `ProductForm.razor` currently receives `Categories` as a `[Parameter]` from its parent page — that parent page call site will also need updating to stop passing categories and let the form load them internally via `MenuApiClient`.
