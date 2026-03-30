# Feature Brief

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Menu` |
| **Feature name** | `GetIngredientsForDropdown` |
| **Short description** | Returns a list of ingredients (id + name) for use in dropdown selectors (e.g. in the Product form). |
| **Reference feature** | `GetCategoriesForDropdown` |
| **Source branch** | `feature/get_ingredients_options` |
| **Target branch** | `feature/get_ingredients_options` |

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
| **Route** | `/api/menu/ingredients/dropdown` |
| **Authorization policy** | `RestaurantManager` |
| **Applies IValidator** | no — no query parameters to validate |
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
| **New DB extension method needed** | yes — add `ForDropdown()` extension on `DbSet<Ingredient>` in `DbExtensions.cs` (returns `IQueryable<Ingredient>` filtered `AsNoTracking`, ordered by `Name`) |

---

## 6) Frontend Details (frontend scope)

| Field | Value |
|---|---|
| **Pages to create or update** | none |
| **Components to create or update** | `ProductForm.razor` — remove `[Parameter] Ingredients` and load ingredients via API in `OnInitializedAsync`; also update `CreateProductPage.razor` to remove the `Ingredients` parameter pass-through |
| **API service to create or update** | `MenuApiClient` — add `GetIngredientsForDropdownAsync()` method and `GetIngredientsForDropdownResponse(Guid Id, string Name)` record |
| **Reference frontend feature** | `ProductForm.razor` pattern for `GetCategoriesForDropdownAsync` call in `OnInitializedAsync` |

---

## 7) Testing Requirements

| Test type | Required | Notes |
|---|---|---|
| Unit tests | no | — |
| Integration tests | yes | Reference `GetCategoriesForDropdownTests` — cover: returns 200 with list of ingredients, returns 200 for authenticated manager, returns 401 for unauthenticated user, returns 403 for non-manager roles (Employee, Customer) |
| Architecture tests | no | — |
| System tests | no | — |

---

## 8) Additional Notes

- Response shape: `GetIngredientsForDropdownResponse(Guid Id, string Name)` — same pattern as `GetCategoriesForDropdownResponse`.
- Handler queries `dbContext.Ingredients.ForDropdown()` (new extension) and projects each ingredient via a `ToResponse()` mapping.
- `ProductForm.razor` currently accepts `[Parameter] public IEnumerable<IngredientOption> Ingredients { get; set; }` — after this feature the form will self-load ingredients from the API, removing the external parameter dependency.
- `CreateProductPage.razor` passes `Ingredients="_ingredients"` to `ProductForm` — this binding must be removed once `ProductForm` self-loads.
