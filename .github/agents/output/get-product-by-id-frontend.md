# Feature plan — GetProductByIdForManage (Frontend)

- **Date**: 2025-07-15
- **Feature**: GetProductByIdForManage — Blazor-side API client method and response type for loading a product by ID
- **Reference**: `GetIngredientByIdResponse` (Blazor), `MenuApiClient.GetIngredientByIdAsync`

---

## 11) Create frontend feature structure

No new folders needed — response type goes into existing `Features/Menu/Products/Responses/` folder.

---

## 14) Create or update API communication services and API Response model

### New file: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/Responses/GetProductByIdForManageResponse.cs`
- `public sealed record GetProductByIdForManageResponse(Guid Id, string Name, string Description, decimal Price, Guid CategoryId, IEnumerable<Guid> IngredientIds)`
- Reference: `GetIngredientByIdResponse`

### Update: `MenuApiClient.cs`
- Add method: `GetProductByIdForManageAsync(Guid id, CancellationToken ct = default)`
  - `GET /api/menu/products/{id}`
  - Returns `GetProductByIdForManageResponse?`
  - Pattern: `httpClient.GetFromJsonAsync<GetProductByIdForManageResponse>($"/api/menu/products/{id}", ct)`
- Reference: `GetIngredientByIdAsync` in `MenuApiClient.cs`

---

## 14) Create or update models, DTOs and mappings

No new UI models needed for this feature — the response is consumed by the `UpdateProduct` feature's `EditProductPage`.

---

## 15) Create or update Blazor components and pages

No component or page changes needed — this feature only provides the backend endpoint and Blazor API client method.

---

## 16) Create Unit tests for Blazor components and services

Skip — no Blazor component logic to test.
