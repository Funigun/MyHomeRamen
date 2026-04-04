# Feature Brief

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Refactor` |
| **Module** | `Menu` |
| **Feature name** | `GetCategoriesByType` |
| **Short backend description** | Replaces `GetCategoriesForDropdown` and `GetCategoriesForManage` endpoints |
| **Short frontend description** | Replaces `CategoriesIndexPage` with `ProductsMaanagementPage` and `IngredientsManagementPage` |
| **Reference feature** | `GetCategoriesForDropdown`, `GetCategoriesForManage` |

---

---
## 2) Scope

| Scope | Include? |
|---|---|
| `backend` — API + Domain + Persistence | yes |
| `frontend` — Blazor Server / WASM | yes |

---

## 3) API Details (backend scope)

Endpoints `GetCategoriesForDropdown` and `GetCategoriesForManage` should be replaced with single `GetCategoriesByType` endpoint
that accepts a int `CategoryType` parameter that should be translated to `CategoryType` enum.

Currently there are two extension methods in `DbExtensions.cs` for filtering categories by type: `ForDropdown()` and `ForManage()`. 
These should be consolidated into a single `ForCategoryType(CategoryType categoryType)` extension method that applies the appropriate filtering and ordering based on the category type.

## 6) Frontend Details (frontend scope)

`ProductsMaanagementPage` and `IngredientsManagementPage` should be created to replace `CategoriesIndexPage`. 
These pages will use the new `GetCategoriesByType` endpoint to load categories based on the type (Product or Ingredient) and display them in a dropdown for selection when creating or managing products and ingredients.

Additionally `CreateCategoryForm` component should now accept a `CategoryType` parameter to determine which type of category is being created (Product or Ingredient) and use the appropriate API endpoint to submit the form data.
Dropdown for CategoryType in this form should be removed due to parameterization.

We also want to update `EmployeeLayout` to include links to both `ProductsMaanagementPage` and `IngredientsManagementPage` for easier navigation.
These links should be added as child elements under `Menu Management` section in the layout.

---

## 7) Testing Requirements

| Test type | Required | Notes |
|---|---|---|
| Unit tests | no | — |
| Integration tests | yes | Reference `GetCategoriesByType` — cover: returns 200 with list of ingredients, returns 200 for authenticated manager, returns 401 for unauthenticated user, returns 403 for non-manager roles (Employee, Customer), remove tests for `GetCategorieForDropdown` and `GetCategoriesForManage` |
| Architecture tests | no | — |
| System tests | no | — |

---
