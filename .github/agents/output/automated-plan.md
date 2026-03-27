Feature implementation plan:
- **Date**: 24.03.2026 22:40
- **Feature**: CreateCategory endpoint for menu categories management

## 1) Create feature folder and structure

```
MyHomeRamen.Api/
??? Menu/
    ??? Features/
        ??? Categories/
            ??? CategoriesGroup.cs
            ??? CreateCategory/
                ??? Models/
                ?   ??? DTOs/
                ?   ?   ??? Mappings.cs
                ?   ??? CreateCategoryRequest.cs
                ?   ??? CreateCategoryResponse.cs
                ??? Policies/
                ?   ??? CreateCategoryValidator.cs
                ??? CreateCategoryEndpoint.cs
                ??? CreateCategoryHandler.cs
```

## 2) Create primitive rules and contracts

Create `CategoryNameValidator` in `MyHomeRamen.Common.Contracts/Menu/Categories/`:
- `AbstractValidator<string>` reusing `CategoryConstants.MinNameLength` / `CategoryConstants.MaxNameLength`
- Exports `MinLength` and `MaxLength` constants matching domain constants
- Rules: `NotEmpty()`, `MinimumLength(MinLength)`, `MaximumLength(MaxLength)`

Create `CategorySortOrderValidator` in `MyHomeRamen.Common.Contracts/Menu/Categories/`:
- `AbstractValidator<int>` reusing `CategoryConstants.MinSortOrder`
- Exports `MinSortOrder` constant
- Rules: `GreaterThanOrEqualTo(MinSortOrder)`

```
MyHomeRamen.Common.Contracts/
??? Menu/
    ??? Categories/
        ??? CategoryNameValidator.cs
        ??? CategorySortOrderValidator.cs
```

## 3) Create models, DTOs and mappings

### CreateCategoryRequest
```csharp
public sealed record CreateCategoryRequest(
    string Name,
    int CategoryType) : IRequest<Guid>;
```
- **Note**: `SortOrder` is NOT part of the request. It is auto-calculated in the handler as `max SortOrder + 1` for the given `CategoryType`.
- `CategoryType` is sent as `int` matching the enum value (`1 = Product`, `2 = Ingredient`).

### CreateCategoryResponse
```csharp
public sealed record CreateCategoryResponse(Guid Id);
```

### Mappings (DTOs/Mappings.cs)
```csharp
internal static class Mappings
{
    public static Category ToDomain(this CreateCategoryRequest request, int nextSortOrder)
    {
        return Category.Create(
            Guid.NewGuid(),
            request.Name,
            nextSortOrder,
            (CategoryType)request.CategoryType);
    }
}
```

## 4) Create IRequestHandler implementation

### CreateCategoryHandler
- Inject `IMenuDbContext`
- Calculate next sort order: query `dbContext.Categories` via `GetNextSortOrderAsync` extension
- Map request to domain via `Mappings.ToDomain(request, nextSortOrder)`
- Add to `dbContext.Categories`
- Call `SaveChangesAsync`
- Return `category.Id.Value`

### DbExtensions addition
Add to `MyHomeRamen.Persistance/Common/DbExtensions.cs`:
```csharp
public static async Task<bool> IsCategoryNameUniqueAsync(
    this IQueryable<Category> query,
    string name,
    CancellationToken cancellationToken = default)
{
    return !await query.AnyAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
}

public static async Task<int> GetNextSortOrderAsync(
    this IQueryable<Category> query,
    CategoryType categoryType,
    CancellationToken cancellationToken = default)
{
    bool any = await query.AnyAsync(c => c.CategoryType == categoryType, cancellationToken);
    if (!any) return CategoryConstants.MinSortOrder;
    return await query.Where(c => c.CategoryType == categoryType)
                      .MaxAsync(c => c.SortOrder, cancellationToken) + 1;
}
```

## 5) Create CategoriesGroup (IGroupEndpoint)

### CategoriesGroup.cs
```csharp
public sealed class CategoriesGroup : IGroupEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithTags("Categories")
                    .WithDescription("Categories management operations")
                    .RequireAuthorization();  // ? mandatory per backend.instructions §3.2
    }
}
```

## 6) Create IEndpoint implementation

### CreateCategoryEndpoint
- GroupName: `"Menu"`
- Route: `"categories"` (resolves to `/api/menu.categories`)
- Method: `MapStandardValidatedPost<CreateCategoryRequest, CreateCategoryResponse>`
- Name: `"CreateCategoryEndpoint"`
- Authorization: `.RequireAuthorization(AuthorizationConfiguration.RestaurantManagerPolicy)`
- Handler returns `Results.Created($"/api/menu/categories/{id}", response)`

### CreateCategoryValidator (Policies/)
FluentValidation `AbstractValidator<CreateCategoryRequest>`:
- `RuleFor(x => x.Name)` ? `.SetValidator(new CategoryNameValidator())`
- `RuleFor(x => x.Name)` ? `.MustAsync(BeUniqueNameAsync)` using `IMenuDbContext` + `IsCategoryNameUniqueAsync`
- `RuleFor(x => x.CategoryType)` ? `.Must(BeValidCategoryType)` checking `Enum.IsDefined((CategoryType)value)`

---

## 7) Create unit tests

### CategoryValidatorsTests (new file)
Location: `MyHomeRamen.UnitTests/MenuModule/Categories/CategoryValidatorsTests.cs`

Tests for `CategoryNameValidator` and `CategorySortOrderValidator` from `Common.Contracts`:
- **CategoryNameValidator**:
  - `CategoryNameValidator_ShouldHaveSameMinLengthAsDomain` — asserts `CategoryNameValidator.MinLength == CategoryConstants.MinNameLength`
  - `CategoryNameValidator_ShouldHaveSameMaxLengthAsDomain` — asserts `CategoryNameValidator.MaxLength == CategoryConstants.MaxNameLength`
  - `CategoryNameValidator_ShouldFail_WhenNameIsEmpty` — validates empty string, asserts error contains `"not empty"`
  - `CategoryNameValidator_ShouldFail_WhenNameIsTooShort` — validates string of length `MinLength - 1`, asserts error contains `"minimum length"`
  - `CategoryNameValidator_ShouldFail_WhenNameIsTooLong` — validates string of length `MaxLength + 1`, asserts error contains `"maximum length"`
  - `CategoryNameValidator_ShouldPass_WhenNameIsValid` — validates string of valid length, asserts valid

- **CategorySortOrderValidator**:
  - `CategorySortOrderValidator_ShouldHaveSameMinSortOrderAsDomain` — asserts `CategorySortOrderValidator.MinSortOrder == CategoryConstants.MinSortOrder`
  - `CategorySortOrderValidator_ShouldFail_WhenSortOrderIsBelowMinimum` — validates `MinSortOrder - 1`, asserts error contains `"greater than or equal to"`
  - `CategorySortOrderValidator_ShouldPass_WhenSortOrderIsValid` — validates `MinSortOrder`, asserts valid

## 8) Create integration tests

### CreateCategoryTests
Location: `MyHomeRamen.IntegrationTests/MenuModule/CreateCategoryTests.cs`

**Prerequisites** (updates to existing files):
- `DataGenerator.cs` — add `InvalidCreateCategoryRequests()` returning `TheoryData<CreateCategoryRequest>` with cases:
  - Name empty, Name too short, Name too long, CategoryType invalid (e.g., `999`)
- `Mappings.cs` — add `ToCreateCategoryRequest(this Category category)` mapping

**Test cases**:
- `CreateCategory_ShouldReturnCreated_ForValidRequest` — Admin auth, valid request, assert 201 + Location header
- `CreateCategory_ShouldReturnUnauthorized_ForNotAuthenticatedUser` — no auth, assert 401
- `CreateCategory_ShouldReturnForbidden_ForNonAdminUser` — Theory with Employee/Customer roles, assert 403
- `CreateCategory_ShouldReturnBadRequest_ForInvalidRequest` — MemberData from `InvalidCreateCategoryRequests()`, assert 400
- `CreateCategory_ShouldReturnBadRequest_ForDuplicateName` — create with existing category name, assert 400
- `CreateCategory_ShouldAssignSequentialSortOrder_ForCategoryType` — create two categories of same type, verify second has SortOrder = first + 1

## 9) Create architecture tests

Architecture tests should be skipped — existing architecture tests already enforce module boundaries and project dependencies generically. No new category-specific architecture rules are needed.

## 10) Create system tests

System tests should be skipped — CreateCategory does not span multiple distributed services. Integration tests provide sufficient coverage.

---

## 11) Create frontend feature structure

```
MyHomeRamen.Blazor/MyHomeRamen.Blazor/
??? Features/
    ??? Menu/
        ??? Categories/
            ??? Components/
            ?   ??? CreateCategoryForm.razor
            ?   ??? CategoryModel.cs
            ?   ??? CategoryValidator.cs
            ??? CreateCategory/
            ?   ??? CreateCategoryRequest.cs
            ??? CategoriesIndex/
                ??? CategoriesIndexPage.razor
```

## 12) Create or update models, DTOs and mappings

### CreateCategoryRequest (Blazor DTO)
Location: `Features/Menu/Categories/CreateCategory/CreateCategoryRequest.cs`
```csharp
public sealed record CreateCategoryRequest(string Name, int CategoryType);
```

### CategoryModel (UI Model)
Location: `Features/Menu/Categories/Components/CategoryModel.cs`
```csharp
public sealed class CategoryModel
{
    public string Name { get; set; } = string.Empty;
    public int CategoryType { get; set; }

    public CreateCategoryRequest ToCreateRequest()
    {
        return new CreateCategoryRequest(Name, CategoryType);
    }
}
```

### CategoryValidator (UI Validator)
Location: `Features/Menu/Categories/Components/CategoryValidator.cs`
- Extends `BaseValidator<CategoryModel>`
- `RuleFor(x => x.Name)` ? `.SetValidator(new CategoryNameValidator())` (from Common.Contracts)
- `RuleFor(x => x.CategoryType)` ? `.Must(v => Enum.IsDefined((CategoryType)v))` with message `"Please select a valid category type."`

## 13) Create or update API communication services

### MenuApiClient update
Add to existing `MenuApiClient.cs`:
```csharp
public async Task<Guid> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken ct = default)
{
    using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/menu.categories", request, ct);
    response.EnsureSuccessStatusCode();
    CreateCategoryResponse? result = await response.Content.ReadFromJsonAsync<CreateCategoryResponse>(ct);
    return result?.Id ?? throw new InvalidOperationException("Failed to deserialize category creation response.");
}
```

Also add response record:
```csharp
public sealed record CreateCategoryResponse(Guid Id);
```

### MenuNavigationService update
Add category routes to existing `MenuNavigationService.cs`:
```csharp
public const string Categories = "/menu/categories";
```
And navigation method:
```csharp
public void ToCategories() => navigation.NavigateTo(Routes.Categories);
```

## 14) Create or update Blazor components and pages

### CreateCategoryForm.razor
Location: `Features/Menu/Categories/Components/CreateCategoryForm.razor`
- MudForm with validation via `CategoryValidator.ValidateValue`
- Fields: `MudTextField` for Name, `MudSelect<int>` for CategoryType (dropdown with "Product" and "Ingredient" options)
- Submit button with busy state pattern
- `[Parameter] public EventCallback<Guid> OnSuccess { get; set; }` — invoked after successful API call
- Injects `MenuApiClient` for API call

### CategoriesIndexPage.razor
Location: `Features/Menu/Categories/CategoriesIndex/CategoriesIndexPage.razor`
- Route: `@page "/menu/categories"`
- Authorization: `@attribute [Authorize(Roles = MenuRoleConstants.Admin)]`
- Layout:
  1. `<MudPaper>` wrapper
  2. Page title "Category Management"
  3. `<CreateCategoryForm>` component at the top with `OnSuccess` callback to show success message
  4. `<MudDivider Class="my-6" />`
  5. `<MudText Typo="Typo.h5">Product Categories</MudText>` section header
  6. `<MudAlert Severity="Severity.Info">Category list coming soon.</MudAlert>` placeholder
  7. `<MudDivider Class="my-6" />`
  8. `<MudText Typo="Typo.h5">Ingredient Categories</MudText>` section header
  9. `<MudAlert Severity="Severity.Info">Category list coming soon.</MudAlert>` placeholder

## 15) Create Unit tests for Blazor components and services

Blazor tests should be skipped — blazor-tests instructions are not yet defined (`TODO` in instructions file). Tests will be added when the testing framework for Blazor is established.

---

## Implementation Order

1. **Common.Contracts** — `CategoryNameValidator`, `CategorySortOrderValidator`
2. **Persistance** — `DbExtensions` additions (`IsCategoryNameUniqueAsync`, `GetNextSortOrderAsync`)
3. **API** — `CategoriesGroup`, `CreateCategory` feature folder (Request, Response, Mappings, Validator, Handler, Endpoint)
4. **Unit Tests** — `CategoryValidatorsTests`
5. **Integration Tests** — `DataGenerator` updates, `Mappings` update, `CreateCategoryTests`
6. **Blazor** — `CreateCategoryRequest`, `CategoryModel`, `CategoryValidator`, `CreateCategoryForm.razor`, `CategoriesIndexPage.razor`, `MenuApiClient` update, `MenuNavigationService` update
