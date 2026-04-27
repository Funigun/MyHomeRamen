# Plan: Delete Product — Tests

## Metadata

**Type:** Feature  
**Tests Affected:** Integration  
**Created:** 2025-07-14

## References

- `MyHomeRamen.IntegrationTests/MenuModule/Ingredients/DeleteIngredientTests.cs` — primary pattern to follow (structure, auth scenarios, DB assertion, HTTP helper usage)
- `MyHomeRamen.IntegrationTests/MenuModule/Common/Data/DataGenerator.cs` — `GeneratedProducts`, `GetRandomProduct()`
- `MyHomeRamen.IntegrationTests/Common/Configuration/HttpClientExtensions.cs` — `CreateDeleteMessage`, `AddAuthorizationHeader`
- `MyHomeRamen.IntegrationTests/Common/WebApiFactory.cs` — `MenuDbContext`, `HttpClient`

---

## Testing Plan

### Step 1: Unit Tests

No unit tests required:
- No new domain entity logic, no validator constants in `MyHomeRamen.Common.Contracts`, no static `{Entity}Validator`.

---

### Step 2: Integration Tests

#### New file

**`MyHomeRamen.IntegrationTests/MenuModule/Products/DeleteProductTests.cs`**

Inject `WebApiFactory` via primary constructor — same as `DeleteIngredientTests`.

#### Test cases

| # | Test method | Scenario | Expected result |
|---|---|---|---|
| 1 | `DeleteProduct_ShouldReturnNoContent_ForValidId` | Valid product ID, authenticated as Admin | `204 No Content`; product no longer exists in DB (verified via `AnyAsync`) |
| 2 | `DeleteProduct_ShouldReturnUnauthorized_ForUnauthenticatedUser` | No auth header | `401 Unauthorized` |
| 3 | `DeleteProduct_ShouldReturnForbidden_ForNonAdminRole` (Theory: Employee, Customer) | Auth header with non-manager role | `403 Forbidden` |
| 4 | `DeleteProduct_ShouldReturnBadRequest_ForNonExistentId` | Random `Guid.NewGuid()` not seeded | `400 Bad Request` |

#### Test case details

**Test 1 — Happy path**
- Seed a standalone product directly on `apiFactory.MenuDbContext` (create via `Product.Create(...)` using `DataGenerator` helpers or building from seeded categories/ingredients).
- Build a `CreateDeleteMessage($"/api/menu/products/{product.Id}").AddAuthorizationHeader(UserRoles.Admin)` request.
- Assert `204 NoContent`.
- Assert `AnyAsync(p => p.Id == product.Id)` returns `false`.

**Test 2 — Unauthenticated**
- Use any `GeneratedProducts.First().Id`.
- Send without `AddAuthorizationHeader`.
- Assert `401 Unauthorized`.

**Test 3 — Forbidden roles (Theory)**
- `[InlineData(UserRoles.Employee)]` and `[InlineData(UserRoles.Customer)]`.
- Assert `403 Forbidden`.

**Test 4 — Non-existent ID**
- Use `Guid.NewGuid()`.
- Authenticate as Admin.
- Assert `400 Bad Request` (validation failure — product does not exist).

---

### Step 3: Integration Tests for Identity module

Not applicable — feature belongs to `Menu` module only.

---

### Step 4: System Tests

Not applicable — no cross-service workflow involved; covered by integration tests.

---

### Step 5: Blazor Tests

Not applicable — the Blazor change is a minimal one-line addition (`await MenuApiClient.DeleteProductAsync(id)`) in an existing try/catch block. The `ProductTable.razor` confirmation dialog and `OnDelete` callback were already present and are covered by the existing UI structure.
