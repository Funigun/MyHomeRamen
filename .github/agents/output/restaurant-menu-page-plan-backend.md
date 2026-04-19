# Feature Implementation Plan — RestaurantMenuPage (Backend)

- **Date**: 2025-07-14
- **Feature**: RestaurantMenuPage — `GetMenuCategories` anonymous endpoint

---

## 1) Create feature folder and structure

```
MyHomeRamen.Api/
└── Menu/
    └── Features/
        └── Categories/
            └── GetMenuCategories/
                ├── GetMenuCategoriesEndpoint.cs
                ├── GetMenuCategoriesHandler.cs
                └── Models/
                    ├── GetMenuCategoriesRequest.cs
                    ├── GetMenuCategoriesResponse.cs
                    └── Mappings.cs
```

Reference: `MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesByType/` (same structure)

---

## 2) Create primitive rules and contracts

Not applicable — no new primitive types or validators needed. The endpoint has no request parameters.

---

## 3) Create models, DTOs and mappings

### `GetMenuCategoriesRequest.cs`
- Empty record implementing the mediator request interface (no parameters needed)
- Pattern: `record GetMenuCategoriesRequest : IRequest<IEnumerable<GetMenuCategoriesResponse>>`

### `GetMenuCategoriesResponse.cs`
- Record with properties: `Guid Id`, `string Name`
- Pattern: match `GetCategoriesByTypeResponse` but simplified (no `CategoryType`, no `Order`)

### `Mappings.cs`
- Static mapping extension from `Category` entity to `GetMenuCategoriesResponse`
- Reference: `GetCategoriesByType/Models/Mappings.cs`

---

## 4) Create IRequestHandler implementation

### `GetMenuCategoriesHandler.cs`
- Implement `IRequestHandler<GetMenuCategoriesRequest, IEnumerable<GetMenuCategoriesResponse>>`
- Inject `MenuDbContext` (or the appropriate DbContext)
- Query: filter categories where `CategoryType == CategoryType.Product`, project to `GetMenuCategoriesResponse`
- Order by `Order` (display order) if the field exists, or by `Name`
- Return `IEnumerable<GetMenuCategoriesResponse>`
- Reference: `GetCategoriesByTypeHandler` — same pattern but hardcoded filter instead of request parameter

---

## 5) Create IGroupedEndpoint implementation (if needed)

Not needed — categories already have an existing group endpoint (`/api/menu/categories`). The new endpoint will be registered under the same group.

---

## 6) Create IEndpoint implementation

### `GetMenuCategoriesEndpoint.cs`
- Implement `IEndpoint`
- Route: `GET /api/menu/categories/menu`
- **No authorization** — use `AllowAnonymous()`
- Map to `GetMenuCategoriesHandler` via mediator
- Return `200 OK` with `IEnumerable<GetMenuCategoriesResponse>`
- Reference: `GetCategoriesByTypeEndpoint` — same pattern but no auth policy and no route parameters

---

## 7) Create unit tests

Not in scope — the handler is a straightforward read-only query with no domain logic. No unit tests justified per brief.

---

## 8) Create integration tests

### File: `MyHomeRamen.IntegrationTests/MenuModule/Categories/GetMenuCategoriesTests.cs`

Reference: `GetCategoriesByTypeTests.cs`

**Test cases:**

1. **`GetMenuCategories_ShouldReturn_OnlyProductCategories`**
   - Seed categories of both `Product` and `Ingredient` types
   - Call `GET /api/menu/categories/menu` (no auth token)
   - Assert only `Product`-type categories are returned
   - Assert response contains correct `Id` and `Name` values

2. **`GetMenuCategories_ShouldReturn_EmptyList_WhenNoProductCategoriesExist`**
   - Seed only `Ingredient`-type categories
   - Call `GET /api/menu/categories/menu`
   - Assert response is `200 OK` with empty list

3. **`GetMenuCategories_ShouldReturn_OK_ForAnonymousUser`**
   - Do not provide any auth token
   - Call `GET /api/menu/categories/menu`
   - Assert `200 OK` (not `401 Unauthorized`)

---

## 9) Create architecture tests (if applicable)

Not in scope — no new architectural patterns introduced.

---

## 10) Create system tests (if applicable)

Not in scope — the feature is a simple read endpoint with no cross-service orchestration.
