# Plan: Product Customization Dialog (Frontend)

## Metadata

**Type:** Feature  
**Layers Affected:** Blazor  
**Created:** 2026-05-05

## References

- Backend endpoint (GET): `get-product-by-id-backend-plan.md` → `GET /api/menu/products/{id}` (AllowAnonymous)
- Backend endpoint (POST): `MyHomeRamen.Api/ShoppingCart/Features/Baskets/AddItemToBasket/AddItemToBasketEndpoint.cs` → `POST /api/shoppingcart/basket/items` (AllowAnonymous)
- Basket quantity rules: `MyHomeRamen.Common.Contracts/Basket/BasketItemQuantityValidator.cs` — Min=1, Max=50
- Dialog pattern: `MyHomeRamen.Blazor/Features/Menu/Categories/Components/CategoryFormDialog.razor`
- Typed HttpClient pattern: `MyHomeRamen.Blazor/Features/Menu/Common/Services/MenuApiClient.cs`
- DI registration: `MyHomeRamen.Blazor/Presentation/ApiDependencyInjection.cs`
- Form model pattern: `MyHomeRamen.Blazor/Features/Menu/Products/Components/ProductModel.cs`
- Form submission pattern: `MyHomeRamen.Blazor/Instructions/blazor.instructions.md` (Form Submission Pattern section)
- Entry point (triggers dialog): `MyHomeRamen.Blazor/Features/Menu/RestaurantMenuPage.razor`

---

> ⚠️ **Backend Pre-requisite: IngredientDto Missing `Id`**
>
> The `GetProductByIdResponse.IngredientDto` in `get-product-by-id-backend-plan.md` is currently defined as:
> ```csharp
> public sealed record IngredientDto(string Name, string Description, decimal Price);
> ```
> However, `AddItemToBasketRequest` requires `IngredientRequestDto(Guid Id, int Quantity)`. The ingredient `Id` is needed to map user selections back to the basket request.
>
> **Action required before implementing the frontend:**
> Extend `IngredientDto` in `MyHomeRamen.Api/Menu/Features/Products/GetProductById/Models/IngredientDto.cs` to include `Guid Id` as the first parameter, and update `Mappings.cs` accordingly.

---

## Implementation Plan

### Step 1: Create Frontend Feature Structure

Create a new `ShoppingCart` module folder alongside existing feature modules:

```
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/ShoppingCart/
├── Common/
│   └── Services/
│       └── ShoppingCartApiClient.cs
└── Basket/
    └── AddItemToBasket/
        ├── Components/
        │   ├── ProductCustomizationDialog.razor
        │   ├── ProductCustomizationModel.cs
        │   ├── IngredientCustomizationModel.cs
        │   └── ProductCustomizationValidator.cs
        ├── Requests/
        │   ├── AddItemToBasketRequest.cs
        │   └── IngredientRequestDto.cs
        └── Responses/
            └── AddItemToBasketResponse.cs
```

Also add to `Features/Menu/Products/Responses/`:
- `GetProductByIdResponse.cs`
- `IngredientDto.cs`

---

### Step 2: Create or Update API Communication Services and Response Models

#### 2.1 Add Blazor-Side Response DTOs for GetProductById

**File:** `Features/Menu/Products/Responses/IngredientDto.cs`

```csharp
public sealed record IngredientDto(Guid Id, string Name, string Description, decimal Price);
```

> Note: This mirrors the backend `GetProductById` response shape and must include `Id` (see pre-requisite above).

**File:** `Features/Menu/Products/Responses/GetProductByIdResponse.cs`

```csharp
public sealed record GetProductByIdResponse(
    Guid Id,
    string Name,
    string Description,
    List<IngredientDto> BaseIngredients,
    List<IngredientDto> CustomIngredients);
```

#### 2.2 Add `GetProductByIdAsync` to `MenuApiClient`

**File:** `Features/Menu/Common/Services/MenuApiClient.cs`

Add method:

```csharp
public async Task<GetProductByIdResponse?> GetProductByIdAsync(Guid id, CancellationToken ct = default)
{
    return await httpClient.GetFromJsonAsync<GetProductByIdResponse>($"/api/menu/products/{id}", ct);
}
```

#### 2.3 Create Blazor-Side DTOs for AddItemToBasket

**File:** `Features/ShoppingCart/Basket/AddItemToBasket/Requests/IngredientRequestDto.cs`

```csharp
public sealed record IngredientRequestDto(Guid Id, int Quantity);
```

**File:** `Features/ShoppingCart/Basket/AddItemToBasket/Requests/AddItemToBasketRequest.cs`

```csharp
public sealed record AddItemToBasketRequest(
    Guid ProductId,
    int Quantity,
    List<IngredientRequestDto> BaseIngredients,
    List<IngredientRequestDto> CustomIngredients,
    string? Comments);
```

**File:** `Features/ShoppingCart/Basket/AddItemToBasket/Responses/AddItemToBasketResponse.cs`

```csharp
public sealed record AddItemToBasketResponse(Guid BasketId, Guid BasketItemId);
```

#### 2.4 Create `ShoppingCartApiClient`

**File:** `Features/ShoppingCart/Common/Services/ShoppingCartApiClient.cs`

- Typed `HttpClient` wrapping the main API.
- Single method `AddItemToBasketAsync(AddItemToBasketRequest request, CancellationToken ct = default) → Task<AddItemToBasketResponse>`:
  - POST to `/api/shoppingcart/basket/items`
  - Call `EnsureSuccessStatusCode()`
  - Deserialize and return `AddItemToBasketResponse`; throw `InvalidOperationException` on null result
- Follow the same pattern as `MenuApiClient`.

#### 2.5 Register `ShoppingCartApiClient` in DI

**File:** `Presentation/ApiDependencyInjection.cs`

Register `ShoppingCartApiClient` targeting the main API service. Since `AddItemToBasket` is `AllowAnonymous` and must support both authenticated users and guests, apply both `AuthHeaderHandler` and `GuestCookieForwardingHandler`:

```csharp
services.AddHttpClient<ShoppingCartApiClient>(client =>
    {
        client.BaseAddress = new Uri($"https+http://{ServiceNames.Api(infrastructurePrefix)}");
    })
    .AddHttpMessageHandler<AuthHeaderHandler>()
    .AddHttpMessageHandler<GuestCookieForwardingHandler>();
```

---

### Step 3: Create Models and Validator

#### 3.1 Create `IngredientCustomizationModel`

**File:** `Features/ShoppingCart/Basket/AddItemToBasket/Components/IngredientCustomizationModel.cs`

Mutable model representing a single ingredient within the customization dialog:

```csharp
public sealed class IngredientCustomizationModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }

    // For base ingredients: always included, quantity ≥ 1.
    // For custom ingredients: IsSelected drives inclusion in the request.
    public bool IsSelected { get; set; } = true;
    public int Quantity { get; set; } = 1;

    public static IngredientCustomizationModel FromDto(IngredientDto dto, bool selectedByDefault = true) =>
        new() { Id = dto.Id, Name = dto.Name, Description = dto.Description, Price = dto.Price, IsSelected = selectedByDefault, Quantity = 1 };

    public IngredientRequestDto ToRequest() => new(Id, Quantity);
}
```

> Base ingredients default to `IsSelected = true` and are always included; the selection toggle is disabled for them.
> Custom ingredients default to `IsSelected = true` (pre-selected) but can be deselected by the user.

#### 3.2 Create `ProductCustomizationModel`

**File:** `Features/ShoppingCart/Basket/AddItemToBasket/Components/ProductCustomizationModel.cs`

Mutable form model holding the full state for the dialog:

```csharp
public sealed class ProductCustomizationModel
{
    public Guid ProductId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public string? Comments { get; set; }

    public List<IngredientCustomizationModel> BaseIngredients { get; init; } = [];
    public List<IngredientCustomizationModel> CustomIngredients { get; init; } = [];

    public static ProductCustomizationModel FromResponse(GetProductByIdResponse response) =>
        new()
        {
            ProductId = response.Id,
            Name = response.Name,
            Description = response.Description,
            Quantity = 1,
            BaseIngredients = response.BaseIngredients.Select(i => IngredientCustomizationModel.FromDto(i, selectedByDefault: true)).ToList(),
            CustomIngredients = response.CustomIngredients.Select(i => IngredientCustomizationModel.FromDto(i, selectedByDefault: true)).ToList(),
        };

    public AddItemToBasketRequest ToRequest() =>
        new(
            ProductId,
            Quantity,
            BaseIngredients.Select(i => i.ToRequest()).ToList(),
            CustomIngredients.Where(i => i.IsSelected).Select(i => i.ToRequest()).ToList(),
            string.IsNullOrWhiteSpace(Comments) ? null : Comments);
}
```

#### 3.3 Create `ProductCustomizationValidator`

**File:** `Features/ShoppingCart/Basket/AddItemToBasket/Components/ProductCustomizationValidator.cs`

- Inherit `AbstractValidator<ProductCustomizationModel>`
- Rules:
  - `Quantity`: GreaterThanOrEqualTo(1), LessThanOrEqualTo(50) — matching `BasketItemQuantityValidator`
  - `RuleForEach(x => x.BaseIngredients)` → `Quantity`: GreaterThanOrEqualTo(1), LessThanOrEqualTo(50)
  - `RuleForEach(x => x.CustomIngredients).When(i => i.IsSelected)` → `Quantity`: GreaterThanOrEqualTo(1), LessThanOrEqualTo(50)
  - `Comments`: MaximumLength matching `BasketItemCommentValidator` from `MyHomeRamen.Common.Contracts`
- Expose `ValidateValue` delegate for MudBlazor `MudForm` binding (follow pattern in `ProductValidator.cs`)

---

### Step 4: Create or Update Blazor Components and Pages

#### 4.1 Create `ProductCustomizationDialog.razor`

**File:** `Features/ShoppingCart/Basket/AddItemToBasket/Components/ProductCustomizationDialog.razor`

Component structure:

- `[CascadingParameter] IMudDialogInstance MudDialog`
- `[Parameter] ProductCustomizationModel Model` — receives pre-populated model from caller
- `[Parameter] EventCallback<AddItemToBasketResponse> OnItemAdded`
- Inject `ShoppingCartApiClient ShoppingCartApiClient`
- Internal state: `MudForm _form`, `bool _isBusy`, `string? _errorMessage`, `ProductCustomizationValidator _validator`

**Markup layout:**

```razor
<MudDialog>
    <DialogContent>
        <MudForm @ref="_form" Model="Model" Validation="_validator.ValidateValue">

            <!-- Product header -->
            <MudText Typo="Typo.h6">@Model.Name</MudText>
            <MudText Typo="Typo.body2" Class="mb-4">@Model.Description</MudText>

            <!-- Product quantity -->
            <MudNumericField T="int"
                             Label="Quantity"
                             @bind-Value="Model.Quantity"
                             For="@(() => Model.Quantity)"
                             Min="1" Max="50"
                             Class="mb-4" />

            <!-- Optional comments -->
            <MudTextField Label="Comments (optional)"
                          @bind-Value="Model.Comments"
                          For="@(() => Model.Comments)"
                          Lines="2"
                          Class="mb-4" />

            <!-- Base ingredients (mandatory, IsSelected always true, toggle disabled) -->
            @if (Model.BaseIngredients.Count > 0)
            {
                <MudText Typo="Typo.subtitle1" Class="mb-2">Base Ingredients</MudText>
                @foreach (IngredientCustomizationModel ingredient in Model.BaseIngredients)
                {
                    <MudGrid AlignItems="AlignItems.Center" Class="mb-2">
                        <MudItem xs="7">
                            <MudText Typo="Typo.body2">@ingredient.Name</MudText>
                            <MudText Typo="Typo.caption" Color="Color.Secondary">@ingredient.Price.ToString("C")</MudText>
                        </MudItem>
                        <MudItem xs="5">
                            <MudNumericField T="int"
                                             @bind-Value="ingredient.Quantity"
                                             Min="1" Max="50"
                                             Label="Qty" />
                        </MudItem>
                    </MudGrid>
                }
            }

            <!-- Custom ingredients (optional, user can toggle inclusion) -->
            @if (Model.CustomIngredients.Count > 0)
            {
                <MudText Typo="Typo.subtitle1" Class="mt-4 mb-2">Customize Ingredients</MudText>
                @foreach (IngredientCustomizationModel ingredient in Model.CustomIngredients)
                {
                    <MudGrid AlignItems="AlignItems.Center" Class="mb-2">
                        <MudItem xs="1">
                            <MudCheckBox @bind-Value="ingredient.IsSelected" />
                        </MudItem>
                        <MudItem xs="6">
                            <MudText Typo="Typo.body2">@ingredient.Name</MudText>
                            <MudText Typo="Typo.caption" Color="Color.Secondary">@ingredient.Price.ToString("C")</MudText>
                        </MudItem>
                        <MudItem xs="5">
                            <MudNumericField T="int"
                                             @bind-Value="ingredient.Quantity"
                                             Min="1" Max="50"
                                             Disabled="@(!ingredient.IsSelected)"
                                             Label="Qty" />
                        </MudItem>
                    </MudGrid>
                }
            }

            @if (_errorMessage is not null)
            {
                <MudAlert Severity="Severity.Error" Class="mt-2">@_errorMessage</MudAlert>
            }

        </MudForm>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => MudDialog.Cancel())" Disabled="_isBusy">Cancel</MudButton>
        <MudButton Color="Color.Primary"
                   Variant="Variant.Filled"
                   OnClick="SubmitAsync"
                   Disabled="_isBusy">
            @if (_isBusy) { <MudProgressCircular Size="Size.Small" Indeterminate="true" /> } else { <span>Add to Basket</span> }
        </MudButton>
    </DialogActions>
</MudDialog>
```

**`@code` block — `SubmitAsync` follows the standard form submission pattern:**

```csharp
private async Task SubmitAsync()
{
    await _form.Validate();
    if (!_form.IsValid) return;

    _isBusy = true;
    _errorMessage = null;
    try
    {
        AddItemToBasketResponse result = await ShoppingCartApiClient.AddItemToBasketAsync(Model.ToRequest());
        await OnItemAdded.InvokeAsync(result);
        MudDialog.Close(result);
    }
    catch (HttpRequestException)
    {
        _errorMessage = "Failed to add item to basket. Please try again.";
    }
    finally
    {
        _isBusy = false;
    }
}
```

#### 4.2 Update `RestaurantMenuPage.razor`

**File:** `Features/Menu/RestaurantMenuPage.razor`

Changes required:

1. **Inject `IDialogService`** at the top of the component.

2. **Replace** the existing inline `MudNumericField` and `MudIconButton` inside each product card `<MudCardContent>` with a single "Customize & Add" button:

   ```razor
   <MudCardActions>
       <MudButton OnClick="@(() => OpenCustomizationDialogAsync(product.Id))"
                  Color="Color.Primary"
                  Variant="Variant.Filled"
                  StartIcon="@Icons.Material.Filled.AddShoppingCart"
                  FullWidth="true">
           Customize & Add
       </MudButton>
   </MudCardActions>
   ```

3. **Add `OpenCustomizationDialogAsync` method** to the `@code` block:

   ```csharp
   private async Task OpenCustomizationDialogAsync(Guid productId)
   {
       GetProductByIdResponse? product = await MenuApiClient.GetProductByIdAsync(productId);
       if (product is null) return;

       ProductCustomizationModel model = ProductCustomizationModel.FromResponse(product);

       DialogParameters<ProductCustomizationDialog> parameters = new();
       parameters.Add(x => x.Model, model);

       DialogOptions options = new() { MaxWidth = MaxWidth.Small, FullWidth = true, CloseOnEscapeKey = true };

       await DialogService.ShowAsync<ProductCustomizationDialog>("Customize Your Order", parameters, options);
   }
   ```

---

### Step 5: Tests

#### Blazor Tests

> Note: `blazor-tests.instructions.md` is currently a placeholder (TODO). Follow general Blazor component testing practices with bUnit when tests are introduced.

No tests are required for this plan at this stage. Once `blazor-tests.instructions.md` is completed, add:

- **`ProductCustomizationDialogTests`**: Verify dialog renders product info, ingredient quantities, validation messages, submit calls `ShoppingCartApiClient`, success closes dialog.
- **`RestaurantMenuPageTests`**: Verify clicking "Customize & Add" fetches product and opens dialog.
