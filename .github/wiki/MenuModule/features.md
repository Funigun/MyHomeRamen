# Patterns

This file contains all information about Menu module features and behaviors.
Treat it as the source of truth in case of what currently exists in the project.

---

## Domain Entities and Value Objects
Note: entity IDs are skipped on purpose since they are forced by design. Entity existence is equal to EntityId existence.

### Entities

| Entity | File path | Description |
|---|---|---|
| `Product` | `MyHomeRamen.Domain/Menu/Products/Product.cs` | Root aggregate for menu products. Holds `Name`, `Description`, `Price`, `ImageUrl`, lists of `BaseIngredients` and `CustomIngredients`, and associated `Categories`. |
| `Ingredient` | `MyHomeRamen.Domain/Menu/Ingredients/Ingredient.cs` | Represents a menu ingredient. Holds `Name`, `Description`, `Price`, and associated `Categories`. |
| `Category` | `MyHomeRamen.Domain/Menu/Categories/Category.cs` | Represents a menu category for grouping products or ingredients. Holds `Name`, `SortOrder`, and `CategoryType`. |
| `User` | `MyHomeRamen.Domain/Menu/Users/User.cs` | Read-side user projection. Holds `Roles`, `Permissions`, and a list of `FavoriteProducts`. |

### Value Objects / Enums

| Type | File path | Description |
|---|---|---|
| `CategoryType` | `MyHomeRamen.Domain/Menu/Categories/CategoryType.cs` | Enum: `Product = 1`, `Ingredient = 2` |

### Entity Methods (factory & mutation)

| Entity | Method | Description |
|---|---|---|
| `Product` | `static Create(ProductId, string name, string description, decimal price, string imageUrl, Collection<Ingredient> baseIngredients, Collection<Ingredient> customIngredients, Collection<Category> categories)` | Creates a product. Validates via `ProductValidator.ValidateProduct`. |
| `Product` | `Update(string name, string description, decimal price, Category category, IEnumerable<Ingredient> ingredients, IEnumerable<Ingredient> customIngredients)` | Updates all mutable fields of the product. Guards: category must not be null. Clears and replaces categories and ingredient collections. Validates via `ProductValidator.ValidateProduct`. |
| `Ingredient` | `static Create(IngredientId, string name, string description, decimal price, IEnumerable<Category> categories)` | Creates an ingredient. Validates via `IngredientValidator.Validate`. |
| `Ingredient` | `Update(string name, string description, decimal price, IEnumerable<Category> categories)` | Updates all mutable fields of the ingredient. Validates via `IngredientValidator.Validate`. |
| `Category` | `static Create(CategoryId, string name, int sortOrder, CategoryType categoryType)` | Creates a category. Validates via `CategoryValidator.Validate`. |
| `Category` | `UpdateSortOrder(int sortOrder)` | Updates the sort order. Validates via `CategoryValidator.ValidateSortOrder`. |
| `User` | `static Create(UserId, List<Role>, List<Permission>)` | Creates the user projection. Validates via `UserValidator.Validate`. |
| `User` | `AddFavoriteProduct(Product product)` | Adds a product to favorites if not already present. |
| `User` | `RemoveFavoriteProduct(Product product)` | Removes a product from favorites. |

---

## Persistence Extension methods

All extensions live in `MyHomeRamen.Persistance/Menu/Extensions/` as `partial class DbExtensions` under the `MyHomeRamen.Persistance.Common` namespace and use the C# 14 `extension` block syntax.

| File | Method | Description |
|---|---|---|
| `ProductDbExtensions.cs` | `IQueryable<Product>.ForCategory(CategoryId)` | `AsNoTracking`. Filters products by category. Includes `BaseIngredients`. |
| `ProductDbExtensions.cs` | `IQueryable<Product>.WithAllIngredients()` | `AsNoTracking`. Includes both `BaseIngredients` and `CustomIngredients`. |
| `ProductDbExtensions.cs` | `IQueryable<Product>.ForManage(string? name, IEnumerable<Guid>? categoryIds, IEnumerable<Guid>? ingredientIds, decimal? priceFrom, decimal? priceTo)` | `AsNoTracking`. Filters products for admin management by name (contains, case-insensitive), category IDs, ingredient IDs (base or custom), and price range. |
| `IngredientDbExtensions.cs` | `IQueryable<Ingredient>.ForDropdown()` | Returns a list ordered by `Name`. Used for dropdown selectors. |
| `IngredientDbExtensions.cs` | `IQueryable<Ingredient>.ForManage(string? name, IEnumerable<Guid>? categoryIds)` | `AsNoTracking`. Filters ingredients by name (contains, case-insensitive) and/or category IDs. Ordered by `Name`. |
| `IngredientDbExtensions.cs` | `IsIngredientNameUniqueAsync(string name, CancellationToken)` | Returns `true` if no other ingredient has the same name (case-insensitive). |
| `IngredientDbExtensions.cs` | `IsIngredientNameUniqueExcludingAsync(string name, IngredientId excludeId, CancellationToken)` | Returns `true` if no ingredient other than the specified one has the same name. Used for update uniqueness checks. |
| `IngredientDbExtensions.cs` | `IsCategoryUsedByIngredientAsync(CategoryId, CancellationToken)` | Returns `true` if any ingredient uses the specified category. Used to guard category deletion. |
| `CategoryDbExtensions.cs` | `IQueryable<Category>.ForCategoryType(CategoryType)` | Returns categories of the specified type, ordered by `SortOrder`. |
| `CategoryDbExtensions.cs` | `IsCategoryNameUniqueAsync(string name, CancellationToken)` | Returns `true` if no category has the same name (case-insensitive). |
| `CategoryDbExtensions.cs` | `GetNextSortOrderAsync(CategoryType, CancellationToken)` | Returns `CategoryConstants.MinSortOrder` if no categories exist for the type; otherwise returns `max(SortOrder) + 1`. |
| `CategoryDbExtensions.cs` | `GetRemainingForResequencingAsync(CategoryType, CategoryId excludeId, CancellationToken)` | Returns all categories of the given type except the excluded one, ordered by `SortOrder`. Used to resequence after deletion. |
| `CategoryDbExtensions.cs` | `CategoryExistsAsync(CategoryId, CancellationToken)` | Returns `true` if a category with the given ID exists. |
| `CategoryDbExtensions.cs` | `IsProductCategoryTypeAsync(CategoryId, CancellationToken)` | Returns `true` if the category exists and has `CategoryType.Product`. |

---

## API Features

All slices are under `MyHomeRamen.Api/Menu/Features/`. Route prefix: `api/menu/`.

### Products — Tag: `Products`

| Slice | Method & Route | Auth | Handler behavior |
|---|---|---|---|
| `CreateProduct` | `POST api/menu/products` | `RestaurantManagerPolicy` | Fetches the category and ingredient/custom-ingredient entities by ID → creates `Product` domain object → persists → returns `201 Created` with `ProductId` |
| `UpdateProduct` | `PUT api/menu/products/{id}` | `RestaurantManagerPolicy` | Fetches existing product, category, and ingredients → calls `Product.Update` → persists → returns `200 Ok` with `UpdateProductResponse` |
| `GetProductsByCategory` | `GET api/menu/products` | `AllowAnonymous` | Filters products by `categoryId` via `ForCategory` (includes base ingredients) → maps to response → returns `200 Ok` |
| `GetProductById` | `GET api/menu/products/{id}` | `AllowAnonymous` | Fetches full product details including base and custom ingredients via `WithAllIngredients` → returns `200 Ok` or `404 NotFound` |
| `GetProductsForManage` | `GET api/menu/products/manage` | `RestaurantManagerPolicy` | Returns a filtered, sorted, and paged list of products via `ForManage` → returns `200 Ok` with paged `GetProductsForManageResponse` |
| `GetProductByIdForManage` | `GET api/menu/products/{id}/manage` | `RestaurantManagerPolicy` | Returns full product details for the management view → returns `200 Ok` with `GetProductByIdForManageResponse` |

### Ingredients — Tag: `Ingredients`

| Slice | Method & Route | Auth | Handler behavior |
|---|---|---|---|
| `CreateIngredient` | `POST api/menu/ingredients` | `RestaurantManagerPolicy` | Creates `Ingredient` domain object → persists → returns `201 Created` with `IngredientId` |
| `UpdateIngredient` | `PUT api/menu/ingredients/{id}` | `RestaurantManagerPolicy` | Fetches existing ingredient → calls `Ingredient.Update` → persists → returns `200 Ok` with `UpdateIngredientResponse` |
| `DeleteIngredient` | `DELETE api/menu/ingredients/{id}` | `RestaurantManagerPolicy` | Validates ingredient exists and is not used by any product → removes → returns `204 NoContent` |
| `GetIngredientById` | `GET api/menu/ingredients/{id}` | `RestaurantManagerPolicy` | Returns full ingredient details → returns `200 Ok` with `GetIngredientByIdResponse` |
| `GetIngredientsForManage` | `GET api/menu/ingredients/manage` | `RestaurantManagerPolicy` | Returns a filtered and paged list of ingredients via `ForManage` (optional name/category filters) → returns `200 Ok` with `GetIngredientsForManageResponse` |
| `GetIngredientsForDropdown` | `GET api/menu/ingredients/dropdown` | `RestaurantManagerPolicy` | Returns all ingredients ordered by name via `ForDropdown` → returns `200 Ok` |

### Categories — Tag: `Categories`

| Slice | Method & Route | Auth | Handler behavior |
|---|---|---|---|
| `CreateCategory` | `POST api/menu/categories` | `RestaurantManagerPolicy` | Determines next `SortOrder` via `GetNextSortOrderAsync` → creates `Category` domain object → persists → returns `201 Created` with `CategoryId` |
| `DeleteCategory` | `DELETE api/menu/categories/{id}` | `RestaurantManagerPolicy` | Fetches category → removes it → resequences remaining categories of the same type via `GetRemainingForResequencingAsync` + `UpdateSortOrder` → persists → returns `204 NoContent` |
| `GetMenuCategories` | `GET api/menu/categories/menu` | `AllowAnonymous` | Returns all `Product`-type categories ordered by `SortOrder` for the public menu page → returns `200 Ok` |
| `GetCategoriesByType` | `GET api/menu/categories/by-type` | `RestaurantManagerPolicy` | Returns categories filtered by `CategoryType` and ordered by `SortOrder` → returns `200 Ok` |
| `UpdateCategoriesOrder` | `PUT api/menu/categories/order` | `RestaurantManagerPolicy` | Batch-updates the `SortOrder` of multiple categories in a single operation → returns `204 NoContent` |
