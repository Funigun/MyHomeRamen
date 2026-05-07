# Plan: Get Product By Id (Public)

## Metadata

**Type:** Feature  
**Layers Affected:** Api, Persistance  
**Created:** 2026-05-05

## References

- Existing GET (single) pattern: `MyHomeRamen.Api/Menu/Features/Products/GetProductByIdForManage/`
- Existing ingredient DTO pattern: `MyHomeRamen.Api/Menu/Features/Products/GetProductsByCategory/Models/ProductIngredientDto.cs`
- Existing existence validator: `MyHomeRamen.Api/Menu/Features/Products/GetProductByIdForManage/Policies/GetProductByIdForManageValidator.cs`
- Generic `GetByIdQuery` extension: `MyHomeRamen.Persistance/Common/RepositoryDbExtensions.cs`
- Database migrations required: **No**

> ⚠️ **Route Conflict Notice:**  
> `GetProductByIdForManageEndpoint` currently maps to `products/{id}`.  
> The new public endpoint will also use `products/{id}` with `AllowAnonymous`.  
> To resolve the conflict, **update `GetProductByIdForManageEndpoint` route from `products/{id}` to `products/{id}/manage`** as part of this implementation.

---

## Implementation Plan

### Step 1: Domain Changes

No domain changes required.  
`Product` already exposes `BaseIngredients` and `CustomIngredients` (`IReadOnlyList<Ingredient>`), and `Ingredient` already carries `Name`, `Description`, and `Price`.

---

### Step 2: Database Changes

No migrations required. No schema changes.

---

### Step 3: Shared Validators

No new validators required in `MyHomeRamen.Common.Contracts`.  
Product ID existence is validated via `GetProductByIdValidator` in the feature's `Policies/` folder (same as `GetProductByIdForManageValidator`).

---

### Step 4: Backend Implementation

#### 4.1 Fix Route Conflict — Update Existing Endpoint

**File:** `MyHomeRamen.Api/Menu/Features/Products/GetProductByIdForManage/GetProductByIdForManageEndpoint.cs`

- Change route argument from `"products/{id}"` to `"products/{id}/manage"`.
- Update `WithName` to `"GetProductByIdForManageEndpoint"` (unchanged).
- Update `WithDescription` to reflect the new route.

**File:** `MyHomeRamen.Blazor\MyHomeRamen.Blazor\Features\Menu\Common\Services\MenuApiClient.cs`
- update `GetProductByIdForManageAsync` route to match the new route (append `/manage`).

**File:** `MyHomeRamen.IntegrationTests\MenuModule\Products\GetProductByIdForManageTests.cs`
- update `EndpointBase` constant to match the new route (append `/manage`).
---

#### 4.2 Create Feature Folder and Structure

Create the following folder structure:

```
MyHomeRamen.Api/Menu/Features/Products/GetProductById/
├── Models/
│   ├── GetProductByIdRequest.cs
│   ├── GetProductByIdResponse.cs
│   ├── IngredientDto.cs
│   └── Mappings.cs
├── Policies/
│   └── GetProductByIdValidator.cs
├── GetProductByIdHandler.cs
└── GetProductByIdEndpoint.cs
```

---

#### 4.3 Create Models, DTOs and Mappings

**File:** `MyHomeRamen.Api/Menu/Features/Products/GetProductById/Models/GetProductByIdRequest.cs`

```csharp
public record struct GetProductByIdRequest : IRequestId<GetProductByIdRequest>, IRequest<GetProductByIdResponse>
{
    public Guid Id { get; set; }
}
```

**File:** `MyHomeRamen.Api/Menu/Features/Products/GetProductById/Models/IngredientDto.cs`

```csharp
public sealed record IngredientDto(string Name, string Description, decimal Price);
```

**File:** `MyHomeRamen.Api/Menu/Features/Products/GetProductById/Models/GetProductByIdResponse.cs`

```csharp
public sealed record GetProductByIdResponse(
    Guid Id,
    string Name,
    string Description,
    List<IngredientDto> BaseIngredients,
    List<IngredientDto> CustomIngredients);
```

**File:** `MyHomeRamen.Api/Menu/Features/Products/GetProductById/Models/Mappings.cs`

- Add `internal static class Mappings` with method `ToResponse(this Product product)`.
- Map `product.BaseIngredients` to `List<IngredientDto>` projecting `Name`, `Description`, `Price`.
- Map `product.CustomIngredients` to `List<IngredientDto>` projecting `Name`, `Description`, `Price`.

```csharp
internal static class Mappings
{
    public static GetProductByIdResponse ToResponse(this Product product) =>
        new(
            product.Id.Value,
            product.Name,
            product.Description,
            product.BaseIngredients.Select(i => new IngredientDto(i.Name, i.Description, i.Price)).ToList(),
            product.CustomIngredients.Select(i => new IngredientDto(i.Name, i.Description, i.Price)).ToList());
}
```

---

#### 4.4 Create Validation Policy

**File:** `MyHomeRamen.Api/Menu/Features/Products/GetProductById/Policies/GetProductByIdValidator.cs`

- Inherit `AbstractValidator<GetProductByIdRequest>`.
- Inject `IMenuDbContext` via primary constructor.
- Rule: `Id` must not be empty.
- Rule: `Id` must correspond to an existing product using `menuDbContext.Products.Exists(p => p.Id == (ProductId)id, ct)`.

Follow the same pattern as `GetProductByIdForManageValidator`.

---

#### 4.5 Create IRequestHandler Implementation

**File:** `MyHomeRamen.Api/Menu/Features/Products/GetProductById/GetProductByIdHandler.cs`

- Inject `IMenuDbContext` via primary constructor.
- Implement `IRequestHandler<GetProductByIdRequest, GetProductByIdResponse>`.
- Cast `request.Id` to `ProductId`.
- Use `dbContext.Products.Include(p => p.BaseIngredients).Include(p => p.CustomIngredients).AsSplitQuery().GetByIdQuery(productId, cancellationToken)`.
- Return `product.ToResponse()`.

Note: No `Include(p => p.Categories)` required — categories are not part of the response.

---

#### 4.6 Create IEndpoint Implementation

**File:** `MyHomeRamen.Api/Menu/Features/Products/GetProductById/GetProductByIdEndpoint.cs`

- Implement `IEndpoint`.
- `GroupName = "Menu"`.
- Map route `"products/{id}"` using `MapStandardValidatedGet<GetProductByIdRequest, GetProductByIdResponse>`.
- `WithName("GetProductByIdEndpoint")`.
- `WithDescription("Returns the full public-facing details of a single product including its base and custom ingredients.")`.
- Apply `.AllowAnonymous()` — this is a public customer-facing endpoint.
- Handler parameter must be named `id` to match the route parameter.

---

### Step 5: Tests

#### Unit Tests

No unit tests required — no new domain logic or validators in `MyHomeRamen.Common.Contracts`.

---

#### Integration Tests

**File:** `MyHomeRamen.IntegrationTests/MenuModule/Products/GetProductByIdTests.cs`

Test cases to implement:

| Test Method | Scenario | Expected Result |
|---|---|---|
| `GetProductById_ShouldReturnOk_ForAnonymousUser` | Anonymous GET to `products/{id}` for a valid seeded product | `200 OK` |
| `GetProductById_ShouldReturnResponseWithCorrectFields` | Anonymous GET for a product with base and custom ingredients | `200 OK`, response `Id`, `Name`, `Description` match seeded entity |
| `GetProductById_ShouldReturnBaseIngredientsWithNameDescriptionAndPrice` | Product with known base ingredients | Response `BaseIngredients` contains correct `Name`, `Description`, `Price` for each ingredient |
| `GetProductById_ShouldReturnCustomIngredientsWithNameDescriptionAndPrice` | Product with known custom ingredients | Response `CustomIngredients` contains correct `Name`, `Description`, `Price` for each ingredient |
| `GetProductById_ShouldReturnBadRequest_ForNonExistentId` | GET with a random, non-seeded `Guid` as admin | `400 Bad Request` |

**Notes:**
- Use `DataGenerator.GeneratedProducts` to get seeded products.
- Use `.AllowAnonymous()` means no auth header is needed for happy-path tests.
- Deserialize response to `GetProductByIdResponse` from `MyHomeRamen.Api`.
- For `GetProductByIdForManageEndpoint` — add a regression test or update the existing `GetProductByIdForManageTests.cs` to use the new `/manage` route suffix.

**File (update):** `MyHomeRamen.IntegrationTests/MenuModule/Products/GetProductByIdForManageTests.cs`

- Update `EndpointBase` constant from `"/api/menu/products"` to `"/api/menu/products"` — change all request URLs that use `{id}` to append `/manage` (i.e., `$"{EndpointBase}/{product.Id.Value}/manage"`).

---

#### System Tests

No system tests required for this feature.
