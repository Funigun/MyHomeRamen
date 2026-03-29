# Feature Brief

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Menu` |
| **Feature name** | `CreateIngredient` |
| **Short description** | Creates a new ingredient with name, description, price, and associated categories. |
| **Reference feature** | `CreateCategory` |
| **Source branch** | `feature/create_ingredient` |
| **Target branch** | `feature/create_ingredient` |

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
| **HTTP method** | `POST` |
| **Route** | `/api/menu/ingredients` |
| **Authorization policy** | `RestaurantManager` |
| **Applies IValidator** | yes — validate name, description, price, and categoryIds |
| **Applies ICachePolicy** | no |
| **Applies IAuthorizationPolicy** | no |

---

## 4) Domain Details (backend scope)

| Field | Value |
|---|---|
| **Aggregate / entity** | `Ingredient` |
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
| **Pages to create or update** | `admin/ingredients` (index page), `admin/ingredients/create` |
| **Components to create or update** | `IngredientForm.razor` |
| **API service to create or update** | `MenuApiClient` — add `CreateIngredientAsync(CreateIngredientRequest request)` method |
| **Reference frontend feature** | `CreateCategoryForm.razor` |

---

## 7) Testing Requirements

| Test type | Required | Notes |
|---|---|---|
| Unit tests | no | — |
| Integration tests | yes | Reference `CreateCategoryTests` — cover: creates ingredient successfully, validates input (name, description, price, categoryIds), returns 201 with created ingredient, returns 400 for invalid input, requires RestaurantManager auth |
| Architecture tests | no | — |
| System tests | no | — |

---

## 8) Additional Notes

- Ingredient requires Name, Description, Price, and Categories (list of CategoryIds for Ingredient type).
- Validators exist: `IngredientNameValidator`, `IngredientDescriptionValidator`, `IngredientPriceValidator`.
- Categories must be of type `CategoryType.Ingredient`.
