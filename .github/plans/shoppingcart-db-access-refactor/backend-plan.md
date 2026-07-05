# Plan: ShoppingCart - Database Access Refactor

## 1. Problem
ShoppingCart module still uses legacy `IShoppingCartDbContext` in Domain exposing `DbSet<T>` and inheriting `IBaseDbContext` transaction methods. Basket and User access is scattered across `BasketDbExtensions`, `UserDbExtensions`, and direct `DbSet` usage in handlers, validators, and integration tests. Need to align with Menu refactor: move context abstraction to Features, implement repository/query/specification pattern, remove direct `DbSet` usage, and drop transaction methods.

## 2. Files to create / modify
| Path | Action | Type | Notes |
|------|--------|------|-------|
| MyHomeRamen.Features\ShoppingCart\Features\Abstractions\IShoppingCartUnitOfWork.cs | Create | | Inherits IUnitOfWork |
| MyHomeRamen.Features\ShoppingCart\Features\Abstractions\IShoppingCartDbContext.cs | Create | | Exposes repository properties, replaces Domain interface |
| MyHomeRamen.Features\ShoppingCart\Features\Baskets\Common\IBasketRepository.cs | Create | | Extends IRepository<Basket, BasketId> |
| MyHomeRamen.Features\ShoppingCart\Features\Baskets\Common\IBasketQuery.cs | Create | | GetForUserAsync, GetByIdForUserAsync, GetByIdForUserWithPaymentAsync, GetByIdForUserWithShippingAsync, GetCurrentBasketSummaryAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Baskets\Common\IBasketSpecification.cs | Create | | GetForUserTrackedAsync, GetByIdForUserTrackedAsync, GetByIdForUserWithPaymentTrackedAsync, GetByIdForUserWithShippingTrackedAsync |
| MyHomeRamen.Features\ShoppingCart\Features\BasketItems\Common\IBasketItemRepository.cs | Create | | Extends IRepository<BasketItem, BasketItemId> |
| MyHomeRamen.Features\ShoppingCart\Features\BasketItems\Common\IBasketItemQuery.cs | Create | | ItemExistsAsync |
| MyHomeRamen.Features\ShoppingCart\Features\BasketItems\Common\IBasketItemSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Products\Common\IProductRepository.cs | Create | | Extends IRepository<Product, ProductId> |
| MyHomeRamen.Features\ShoppingCart\Features\Products\Common\IProductQuery.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Products\Common\IProductSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Ingredients\Common\IIngredientRepository.cs | Create | | Extends IRepository<Ingredient, IngredientId> |
| MyHomeRamen.Features\ShoppingCart\Features\Ingredients\Common\IIngredientQuery.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Ingredients\Common\IIngredientSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Users\Common\IUserRepository.cs | Create | | Extends IRepository<User, UserId> |
| MyHomeRamen.Features\ShoppingCart\Features\Users\Common\IUserQuery.cs | Create | | FindByIdAsync, GetUserIdAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Users\Common\IUserSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Roles\Common\IRoleRepository.cs | Create | | Extends IRepository<Role, RoleId> |
| MyHomeRamen.Features\ShoppingCart\Features\Roles\Common\IRoleQuery.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Roles\Common\IRoleSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Permissions\Common\IPermissionRepository.cs | Create | | Extends IRepository<Permission, PermissionId> |
| MyHomeRamen.Features\ShoppingCart\Features\Permissions\Common\IPermissionQuery.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Permissions\Common\IPermissionSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\ShoppingCart\Features\PaymentDetails\Common\IPaymentDetailsRepository.cs | Create | | Extends IRepository<PaymentDetails, PaymentDetailsId> |
| MyHomeRamen.Features\ShoppingCart\Features\PaymentDetails\Common\IPaymentDetailsQuery.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\ShoppingCart\Features\PaymentDetails\Common\IPaymentDetailsSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\ShoppingCart\Features\ShippingDetails\Common\IShippingDetailsRepository.cs | Create | | Extends IRepository<ShippingDetails, ShippingDetailsId> |
| MyHomeRamen.Features\ShoppingCart\Features\ShippingDetails\Common\IShippingDetailsQuery.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\ShoppingCart\Features\ShippingDetails\Common\IShippingDetailsSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Persistance\ShoppingCart\ShoppingCartDbContext.cs | Modify | | Make partial, remove Begin/Commit/RollbackTransaction, implement IShoppingCartDbContext, add repository properties |
| MyHomeRamen.Persistance\ShoppingCart\Baskets\BasketRepository.cs | Create | | Implements IBasketRepository |
| MyHomeRamen.Persistance\ShoppingCart\Baskets\BasketQuery.cs | Create | | Implements IBasketQuery |
| MyHomeRamen.Persistance\ShoppingCart\Baskets\BasketSpecification.cs | Create | | Implements IBasketSpecification |
| MyHomeRamen.Persistance\ShoppingCart\BasketItems\BasketItemRepository.cs | Create | | Implements IBasketItemRepository |
| MyHomeRamen.Persistance\ShoppingCart\BasketItems\BasketItemQuery.cs | Create | | Implements IBasketItemQuery |
| MyHomeRamen.Persistance\ShoppingCart\BasketItems\BasketItemSpecification.cs | Create | | Implements IBasketItemSpecification |
| MyHomeRamen.Persistance\ShoppingCart\Products\ProductRepository.cs | Create | | Implements IProductRepository |
| MyHomeRamen.Persistance\ShoppingCart\Products\ProductQuery.cs | Create | | Implements IProductQuery |
| MyHomeRamen.Persistance\ShoppingCart\Products\ProductSpecification.cs | Create | | Implements IProductSpecification |
| MyHomeRamen.Persistance\ShoppingCart\Ingredients\IngredientRepository.cs | Create | | Implements IIngredientRepository |
| MyHomeRamen.Persistance\ShoppingCart\Ingredients\IngredientQuery.cs | Create | | Implements IIngredientQuery |
| MyHomeRamen.Persistance\ShoppingCart\Ingredients\IngredientSpecification.cs | Create | | Implements IIngredientSpecification |
| MyHomeRamen.Persistance\ShoppingCart\Users\UserRepository.cs | Create | | Implements IUserRepository |
| MyHomeRamen.Persistance\ShoppingCart\Users\UserQuery.cs | Create | | Implements IUserQuery |
| MyHomeRamen.Persistance\ShoppingCart\Users\UserSpecification.cs | Create | | Implements IUserSpecification |
| MyHomeRamen.Persistance\ShoppingCart\Roles\RoleRepository.cs | Create | | Implements IRoleRepository |
| MyHomeRamen.Persistance\ShoppingCart\Roles\RoleQuery.cs | Create | | Implements IRoleQuery |
| MyHomeRamen.Persistance\ShoppingCart\Roles\RoleSpecification.cs | Create | | Implements IRoleSpecification |
| MyHomeRamen.Persistance\ShoppingCart\Permissions\PermissionRepository.cs | Create | | Implements IPermissionRepository |
| MyHomeRamen.Persistance\ShoppingCart\Permissions\PermissionQuery.cs | Create | | Implements IPermissionQuery |
| MyHomeRamen.Persistance\ShoppingCart\Permissions\PermissionSpecification.cs | Create | | Implements IPermissionSpecification |
| MyHomeRamen.Persistance\ShoppingCart\PaymentDetails\PaymentDetailsRepository.cs | Create | | Implements IPaymentDetailsRepository |
| MyHomeRamen.Persistance\ShoppingCart\PaymentDetails\PaymentDetailsQuery.cs | Create | | Implements IPaymentDetailsQuery |
| MyHomeRamen.Persistance\ShoppingCart\PaymentDetails\PaymentDetailsSpecification.cs | Create | | Implements IPaymentDetailsSpecification |
| MyHomeRamen.Persistance\ShoppingCart\ShippingDetails\ShippingDetailsRepository.cs | Create | | Implements IShippingDetailsRepository |
| MyHomeRamen.Persistance\ShoppingCart\ShippingDetails\ShippingDetailsQuery.cs | Create | | Implements IShippingDetailsQuery |
| MyHomeRamen.Persistance\ShoppingCart\ShippingDetails\ShippingDetailsSpecification.cs | Create | | Implements IShippingDetailsSpecification |
| MyHomeRamen.Persistance\DependencyInjection.cs | Modify | | Register IShoppingCartDbContext, IShoppingCartUnitOfWork, and all aggregate repository interfaces |
| MyHomeRamen.Worker.DatabaseInitializer\DbInitializerJob.cs | Modify | | Move shoppingCartDbContext from IBaseDbContext dictionary to IUnitOfWork dictionary |
| MyHomeRamen.Worker.MessagesHandler\ShoppingCart\ShoppingCartUserRegisteredHandler.cs | Modify | | Use dbContext.User.Exists and dbContext.Role.Query().GetByNameWithPermissions |
| MyHomeRamen.Worker.MessagesHandler\ShoppingCart\ShoppingCartGuestRegisteredHandler.cs | Modify | | Use dbContext.User.Add and dbContext.Basket.Add |
| MyHomeRamen.Features\ShoppingCart\Features\Baskets\AddItemToBasket\AddItemToBasketHandler.cs | Modify | | Use dbContext.User.Query().FindByIdAsync, dbContext.Basket.Specification().GetForUserTrackedAsync, dbContext.Product.Add, dbContext.Ingredient.AddRange, dbContext.BasketItem.Add |
| MyHomeRamen.Features\ShoppingCart\Features\Baskets\ClearBasket\ClearBasketHandler.cs | Modify | | Use dbContext.Basket.Specification().GetByIdForUserTrackedAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Baskets\ClearBasket\ClearBasketValidationPolicy.cs | Modify | | Use dbContext.Basket.Query().GetByIdForUserAsync or Repository.Exists |
| MyHomeRamen.Features\ShoppingCart\Features\Baskets\DeleteBasketItem\DeleteBasketItemHandler.cs | Modify | | Use dbContext.Basket.Specification().GetByIdForUserTrackedAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Baskets\DeleteBasketItem\DeleteBasketItemValidationPolicy.cs | Modify | | Use dbContext.Basket.Query().GetByIdForUserAsync and dbContext.BasketItem.Query().ItemExistsAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Baskets\GetCurrentBasketDetails\GetCurrentBasketDetailsHandler.cs | Modify | | Use dbContext.Basket.Query().GetForUserAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Baskets\GetCurrentBasketSummary\GetCurrentBasketSummaryHandler.cs | Modify | | Use dbContext.User.Query().GetByIdAsync and dbContext.Basket.Query().GetCurrentBasketSummaryAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Baskets\GetPaymentDetails\GetPaymentDetailsHandler.cs | Modify | | Use dbContext.Basket.Query().GetByIdForUserWithPaymentAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Baskets\GetPaymentDetails\GetPaymentDetailsValidationPolicy.cs | Modify | | Use dbContext.Basket.Query().GetByIdForUserAsync or Repository.Exists |
| MyHomeRamen.Features\ShoppingCart\Features\Baskets\GetShippingDetails\GetShippingDetailsHandler.cs | Modify | | Use dbContext.Basket.Query().GetByIdForUserWithShippingAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Baskets\GetShippingDetails\GetShippingDetailsValidationPolicy.cs | Modify | | Use dbContext.Basket.Query().GetByIdForUserAsync or Repository.Exists |
| MyHomeRamen.Features\ShoppingCart\Features\Baskets\UpdatePaymentDetails\UpdatePaymentDetailsHandler.cs | Modify | | Use dbContext.Basket.Specification().GetByIdForUserWithPaymentTrackedAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Baskets\UpdatePaymentDetails\UpdatePaymentDetailsValidationPolicy.cs | Modify | | Use dbContext.Basket.Specification().GetByIdForUserTrackedAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Baskets\UpdateShippingDetails\UpdateShippingDetailsHandler.cs | Modify | | Use dbContext.Basket.Specification().GetByIdForUserWithShippingTrackedAsync |
| MyHomeRamen.Features\ShoppingCart\Features\Baskets\UpdateShippingDetails\UpdateShippingDetailsValidationPolicy.cs | Modify | | Use dbContext.Basket.Specification().GetByIdForUserTrackedAsync |
| MyHomeRamen.IntegrationTests\Common\WebApiFactory.cs | Modify | | Expose IShoppingCartDbContext instead of ShoppingCartDbContext |
| MyHomeRamen.IntegrationTests\ShoppingCartModule\Common\Data\ShoppingCartDataSeeder.cs | Modify | | Use repository Add/AddRange methods |
| MyHomeRamen.IntegrationTests\ShoppingCartModule\Common\Data\ShoppingCartDbContextExtensions.cs | Delete | | Logic moved to UserQuery.GetUserIdAsync |
| MyHomeRamen.IntegrationTests\ShoppingCartModule\Baskets\AddItemToBasketTests.cs | Modify | | Use dbContext.User.Query().GetUserIdAsync |
| MyHomeRamen.IntegrationTests\ShoppingCartModule\Baskets\ClearBasketTests.cs | Modify | | Use dbContext.User.Query().GetUserIdAsync |
| MyHomeRamen.IntegrationTests\ShoppingCartModule\Baskets\GetCurrentBasketDetailsTests.cs | Modify | | Use IShoppingCartDbContext and repository methods |
| MyHomeRamen.IntegrationTests\ShoppingCartModule\Baskets\GetCurrentBasketSummaryTests.cs | Modify | | Use dbContext.User.Query().GetUserIdAsync |
| MyHomeRamen.IntegrationTests\ShoppingCartModule\Baskets\GetPaymentDetailsTests.cs | Modify | | Use IShoppingCartDbContext and repository methods |
| MyHomeRamen.IntegrationTests\ShoppingCartModule\Baskets\GetShippingDetailsTests.cs | Modify | | Use IShoppingCartDbContext and repository methods |
| MyHomeRamen.Domain\ShoppingCart\Database\IShoppingCartDbContext.cs | Delete | | Replaced by Features abstraction |
| MyHomeRamen.Features\ShoppingCart\Features\Common\BasketDbExtensions.cs | Delete | | Logic moved to BasketQuery/BasketSpecification |
| MyHomeRamen.Features\ShoppingCart\Features\Common\UserDbExtensions.cs | Delete | | Logic moved to UserQuery |

## 3. Domain changes
- Delete `MyHomeRamen.Domain.ShoppingCart.Database.IShoppingCartDbContext`.
- No entity changes.
- Migration needed: no.

## 4. Persistance extensions
- `BasketQuery.GetForUserAsync(UserId, ct)` returns `Basket?` with items/products/ingredients (no tracking).
- `BasketQuery.GetByIdForUserAsync(BasketId, UserId, ct)` returns `Basket?` (no tracking).
- `BasketQuery.GetByIdForUserWithPaymentAsync(BasketId, UserId, ct)` returns `Basket?` with payment details (no tracking).
- `BasketQuery.GetByIdForUserWithShippingAsync(BasketId, UserId, ct)` returns `Basket?` with shipping details (no tracking).
- `BasketQuery.GetCurrentBasketSummaryAsync(Guid userId, ct)` returns `BasketSummary?` or `Basket?` mapped to response.
- `BasketSpecification.GetForUserTrackedAsync(UserId, ct)` returns `Basket` tracked.
- `BasketSpecification.GetByIdForUserTrackedAsync(BasketId, UserId, ct)` returns `Basket` tracked.
- `BasketSpecification.GetByIdForUserWithPaymentTrackedAsync(BasketId, UserId, ct)` returns `Basket` tracked with payment details.
- `BasketSpecification.GetByIdForUserWithShippingTrackedAsync(BasketId, UserId, ct)` returns `Basket` tracked with shipping details.
- `BasketItemQuery.ItemExistsAsync(UserId, BasketItemId, BasketId, ct)` returns bool.
- `UserQuery.FindByIdAsync(UserId, ct)` returns `User?` tracked.
- `UserQuery.GetUserIdAsync(bool isGuest, ct)` returns `UserId`.
- `ProductRepository.Add(Product)` and `IngredientRepository.AddRange(IEnumerable<Ingredient>)` used by `AddItemToBasketHandler`.
- `PaymentDetails` and `ShippingDetails` expose standard `ByIdAsync` on Query/Specification and repository CRUD for consistency.

## 5. API details
- `IShoppingCartDbContext` exposes properties: `IBasketRepository Basket`, `IBasketItemRepository BasketItem`, `IProductRepository Product`, `IIngredientRepository Ingredient`, `IUserRepository User`, `IRoleRepository Role`, `IPermissionRepository Permission`, `IPaymentDetailsRepository PaymentDetails`, `IShippingDetailsRepository ShippingDetails`.
- `AddItemToBasketHandler` loads user via `dbContext.User.Query().FindByIdAsync(userId, ct)`, loads or creates basket via `dbContext.Basket.Specification().GetForUserTrackedAsync(userId, ct)`, adds product/ingredients/basket item via repositories, then `dbContext.SaveChangesAsync`.
- `ClearBasketHandler` loads basket via `dbContext.Basket.Specification().GetByIdForUserTrackedAsync(...)`, calls `basket.Clear()`, then `dbContext.SaveChangesAsync`.
- `DeleteBasketItemHandler` loads basket via `dbContext.Basket.Specification().GetByIdForUserTrackedAsync(...)`, calls `basket.RemoveItem(...)`, then `dbContext.SaveChangesAsync`.
- `GetCurrentBasketDetailsHandler` uses `dbContext.Basket.Query().GetForUserAsync(userId, ct)`.
- `GetCurrentBasketSummaryHandler` validates user via `dbContext.User.Query().GetByIdAsync(...)` and loads summary via `dbContext.Basket.Query().GetCurrentBasketSummaryAsync(...)`.
- `GetPaymentDetailsHandler` uses `dbContext.Basket.Query().GetByIdForUserWithPaymentAsync(...)`.
- `GetShippingDetailsHandler` uses `dbContext.Basket.Query().GetByIdForUserWithShippingAsync(...)`.
- `UpdatePaymentDetailsHandler` uses `dbContext.Basket.Specification().GetByIdForUserWithPaymentTrackedAsync(...)`.
- `UpdateShippingDetailsHandler` uses `dbContext.Basket.Specification().GetByIdForUserWithShippingTrackedAsync(...)`.
- Validation policies use Query/Specification equivalents instead of `IQueryable` extensions.

## 6. Tests
- Unit tests: none affected (no domain logic changes).
- Integration tests: update `WebApiFactory` to expose `IShoppingCartDbContext`, update `ShoppingCartDataSeeder` and all ShoppingCart test classes to use repository/query methods instead of direct `DbSet` access.
- Delete `ShoppingCartDbContextExtensions` and replace `GetUserId` calls with `dbContext.User.Query().GetUserIdAsync`.
- Verify all ShoppingCart integration tests still pass.

## 7. Risks / decisions for human approval
- `BasketDbExtensions` currently expose `IQueryable<Basket>` for both tracking and no-tracking scenarios. Refactor collapses these into explicit async methods returning entities/DTOs. Confirm the method signatures above cover all current usage.
- `GetCurrentBasketSummaryAsync` can return either a domain `Basket` mapped in handler or a projected DTO. Prefer projected DTO if response mapping is complex; otherwise return `Basket`.
- Integration tests currently use concrete `ShoppingCartDbContext` and direct `DbSet` manipulation. This is the largest consumer of changes; consider doing this module last or in isolation.

## 8. Out of scope
- Adding new shopping cart features or changing basket business rules.
- Refactoring other modules.
- Blazor frontend changes.
