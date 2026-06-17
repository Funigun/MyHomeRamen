# Patterns

This file contains all information about ShoppingCart module features and behaviors.
Treat it as the source of truth in case of what currently exists in the project.

---

## Domain Entities and Value Objects
Note: entity IDs are skipped on purpose since they are forced by design. Entity existence is equal to EntityId existence.

### Entities

| Entity | File path | Description |
|---|---|---|
| `Basket` | `MyHomeRamen.Domain/ShoppingCart/Baskets/Basket.cs` | Root aggregate. Belongs to a `User`, holds a list of `BasketItem`s and a `BasketStatus`. Enforces max items limit. |
| `BasketItem` | `MyHomeRamen.Domain/ShoppingCart/BasketItems/BasketItem.cs` | Represents one line in the basket. References a snapshot `Product`, holds `Quantity`, calculated `Price`, and optional `Comment`. Price is auto-calculated as `Product.TotalPrice * Quantity` on creation. |
| `Product` | `MyHomeRamen.Domain/ShoppingCart/Products/Product.cs` | Snapshot of a menu product at basket-add time. Has `OriginalId` reference back to the menu product. Holds `BaseIngredients` and `CustomIngredients`. `TotalPrice` is calculated: base price + extra base ingredient quantities + custom ingredient totals. |
| `Ingredient` | `MyHomeRamen.Domain/ShoppingCart/Ingredients/Ingredient.cs` | Snapshot of a menu ingredient. Has `OriginalId` reference. Holds `Name`, `Description`, `Price`, `Quantity`. |
| `User` | `MyHomeRamen.Domain/ShoppingCart/Users/User.cs` | Read-side user projection. Holds `Roles`, `Permissions`, and `IsGuest` flag. |

### Value Objects / Enums

| Type | File path | Description |
|---|---|---|
| `BasketStatus` | `MyHomeRamen.Domain/ShoppingCart/Baskets/BasketStatus.cs` | Enum: `Active`, `PendingOrder`, `CheckedOut`, `Abandoned`, `Expired` |

### Entity Methods (factory & mutation)

| Entity | Method | Description |
|---|---|---|
| `Basket` | `static Create(BasketId, User)` | Creates an active basket for a user. Validates user is not null. |
| `Basket` | `AddItem(BasketItem)` | Adds an item to the basket. Guards: item not null, max items limit not exceeded (`BasketConstants.MaxProductsCount`). |
| `BasketItem` | `static Create(BasketItemId, Product, int quantity, decimal price, string? comment)` | Creates an item. Auto-sets `Price = Product.TotalPrice * Quantity`. Validates: product not null, quantity ≥ min, price ≥ 0. |
| `Product` | `static Create(ProductId, ProductId originalId, string name, string description, decimal price, string imageUrl, List<Ingredient> baseIngredients, List<Ingredient> customIngredients)` | Creates a product snapshot. Validates name/description length, price range, ingredients. Calls `CalculateTotalPrice()`. |
| `Ingredient` | `static Create(IngredientId, IngredientId originalId, string name, string description, decimal price, int quantity)` | Creates an ingredient snapshot. Validates name/description length and price range. |
| `User` | `static Create(UserId, List<Role>, List<Permission>, bool isGuest = false)` | Creates the user projection. |

---

## Persistence Extension methods

All extensions live in `MyHomeRamen.Persistance/ShoppingCart/Extensions/` as `partial class DbExtensions` under the `MyHomeRamen.Persistance.Common` namespace and use the C# 14 `extension` block syntax.

| File | Method | Description |
|---|---|---|
| `BasketDbExtensions.cs` | `IQueryable<Basket>.ForUser(UserId)` | `AsNoTracking`. Filters active baskets for user. Includes full item → product → base/custom ingredients graph. |
| `BasketDbExtensions.cs` | `IQueryable<Basket>.ForUserTracked(UserId)` | Tracked (no `AsNoTracking`). Filters active baskets for user. No includes. Used for write operations. |
| `BasketDbExtensions.cs` | `GetCurrentBasketSummary(this IQueryable<Basket>, Guid userId)` | `AsNoTracking`. Filters active basket for user. Includes items → product only (no ingredients). |
| `UserDbExtensions.cs` | `IQueryable<User>.FindByIdAsync(UserId, CancellationToken)` | Returns `User?` by id. `AsNoTracking` not applied — default tracking. |
| `BasketDbExtensions` | `Task<bool> ItemExistsQuery(this IQueryable<Basket>, UserId, BasketItemId, BasketId, CancellationToken)` | Checks if a basket item with given id exists in the specified active basket of the user. |
| `BasketDbExtensions` | `IQueryable<Basket>.GetByIdForUserTracked(BasketId, UserId)` | Tracked. Filters by id, user, active status. Includes Items. |
| `BasketDbExtensions` | `IQueryable<Basket>.GetByIdForUser(BasketId, UserId)` | `AsNoTracking`. Filters by id, user, active status. Includes Items. |

---

## API Features

All slices are under `MyHomeRamen.Api/ShoppingCart/Features/Baskets/`. Route prefix: `api/shoppingcart/`. Tag: `Baskets`.

| Slice | Method & Route | Auth | Handler behavior | Produced Event |
|---|---|---|---|---|
| `AddItemToBasket` | `POST api/shoppingcart/basket/items` | `AllowAnonymous` | Resolves current user → fetches or creates active `Basket` → calls `IMenuService` to get product+ingredient snapshots → creates `Product`, `Ingredient`, `BasketItem` domain objects → `Basket.AddItem` → persists all → returns `201 Created` with `BasketId` + `BasketItemId` | None |
| `GetCurrentBasketDetails` | `GET api/shoppingcart/basket/summary` | `AllowAnonymous` | Fetches active basket via `ForUser` (full graph: items → product → ingredients) → maps to response → returns `200 Ok` or `400 BadRequest` if no active basket | None |
| `GetCurrentBasketSummary` | `GET api/shoppingcart/baskets` | `AllowAnonymous` | Validates user exists and guest/auth state is consistent → fetches active basket via `GetCurrentBasketSummary` (items → product only) → maps to response → returns `200 Ok` | None |
| `DeleteBasketItem` | `DELETE api/shoppingcart/baskets/{basketId}/items/{basketItemId}` | `AllowAnonymous` | Builds `DeleteBasketItemCommand(BasketId, BasketItemId)` → dispatches to handler → returns `204 NoContent` | None |
| `ClearBasket` | `DELETE api/shoppingcart/baskets/{basketId}` | `AllowAnonymous` | Builds `ClearBasketCommand(BasketId)` → dispatches to handler → returns `204 NoContent` | None |
| `GetShippingDetails` | `GET api/shopping-cart/{id}/shipping-details` | `AllowAnonymous` | Dispatches `GetShippingDetailsQuery(BasketId, UserId)` → handler fetches shipping details → returns `200 Ok` with `ShippingDetailsResponse` or `400 BadRequest` | None |
| `UpdateShippingDetails` | `PUT api/shopping-cart/{id}/update-shipping-details` | `AllowAnonymous` | Dispatches `UpdateShippingDetailsCommand(BasketId, UserId, request)` → handler updates shipping details → returns `200 Ok` or `400 BadRequest` | None |
| `GetPaymentDetails` | `GET api/shopping-cart/{id}/payment-details` | `AllowAnonymous` | Dispatches `GetPaymentDetailsQuery(BasketId, UserId)` → handler fetches payment details → returns `200 Ok` with `PaymentDetailsResponse` or `400 BadRequest` | None |
| `UpdatePaymentDetails` | `PUT api/shopping-cart/{id}/update-payment-details` | `AllowAnonymous` | Dispatches `UpdatePaymentDetailsCommand(BasketId, UserId, request)` → handler updates payment details → returns `200 Ok` or `400 BadRequest` | None |