# Plan: Payments - Database Access Refactor

## 1. Problem
Payments module still uses legacy `IPaymentsDbContext` in Domain exposing `DbSet<T>` and inheriting `IBaseDbContext` transaction methods. Need to align with Menu refactor: move context abstraction to Features, implement repository/query/specification pattern, remove direct `DbSet` usage, and drop transaction methods.

## 2. Files to create / modify
| Path | Action | Type | Notes |
|------|--------|------|-------|
| MyHomeRamen.Features\Payments\Features\Abstractions\IPaymentsUnitOfWork.cs | Create | | Inherits IUnitOfWork |
| MyHomeRamen.Features\Payments\Features\Abstractions\IPaymentsDbContext.cs | Create | | Exposes repository properties, replaces Domain interface |
| MyHomeRamen.Features\Payments\Features\PaymentMethods\Common\IPaymentMethodRepository.cs | Create | | Extends IRepository<PaymentMethod, PaymentMethodId> |
| MyHomeRamen.Features\Payments\Features\PaymentMethods\Common\IPaymentMethodQuery.cs | Create | | GetAvailableMethodsAsync, GetByIdAsync |
| MyHomeRamen.Features\Payments\Features\PaymentMethods\Common\IPaymentMethodSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Payments\Features\PaymentChannels\Common\IPaymentChannelRepository.cs | Create | | Extends IRepository<PaymentChannel, PaymentChannelId> |
| MyHomeRamen.Features\Payments\Features\PaymentChannels\Common\IPaymentChannelQuery.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Payments\Features\PaymentChannels\Common\IPaymentChannelSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Payments\Features\PaymentGateways\Common\IPaymentGatewayRepository.cs | Create | | Extends IRepository<PaymentGateway, PaymentGatewayId> |
| MyHomeRamen.Features\Payments\Features\PaymentGateways\Common\IPaymentGatewayQuery.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Payments\Features\PaymentGateways\Common\IPaymentGatewaySpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Payments\Features\Orders\Common\IOrderRepository.cs | Create | | Extends IRepository<Order, OrderId> |
| MyHomeRamen.Features\Payments\Features\Orders\Common\IOrderQuery.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Payments\Features\Orders\Common\IOrderSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Payments\Features\Users\Common\IUserRepository.cs | Create | | Extends IRepository<User, UserId> |
| MyHomeRamen.Features\Payments\Features\Users\Common\IUserQuery.cs | Create | | ExistsAsync(UserId) |
| MyHomeRamen.Features\Payments\Features\Users\Common\IUserSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Payments\Features\Roles\Common\IRoleRepository.cs | Create | | Extends IRepository<Role, RoleId> |
| MyHomeRamen.Features\Payments\Features\Roles\Common\IRoleQuery.cs | Create | | GetByNameWithPermissionsAsync |
| MyHomeRamen.Features\Payments\Features\Roles\Common\IRoleSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Payments\Features\Permissions\Common\IPermissionRepository.cs | Create | | Extends IRepository<Permission, PermissionId> |
| MyHomeRamen.Features\Payments\Features\Permissions\Common\IPermissionQuery.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Payments\Features\Permissions\Common\IPermissionSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Persistance\Payments\PaymentsDbContext.cs | Modify | | Make partial, remove Begin/Commit/RollbackTransaction, implement IPaymentsDbContext, add repository properties |
| MyHomeRamen.Persistance\Payments\PaymentMethods\PaymentMethodRepository.cs | Create | | Implements IPaymentMethodRepository |
| MyHomeRamen.Persistance\Payments\PaymentMethods\PaymentMethodQuery.cs | Create | | Implements IPaymentMethodQuery |
| MyHomeRamen.Persistance\Payments\PaymentMethods\PaymentMethodSpecification.cs | Create | | Implements IPaymentMethodSpecification |
| MyHomeRamen.Persistance\Payments\PaymentChannels\PaymentChannelRepository.cs | Create | | Implements IPaymentChannelRepository |
| MyHomeRamen.Persistance\Payments\PaymentChannels\PaymentChannelQuery.cs | Create | | Implements IPaymentChannelQuery |
| MyHomeRamen.Persistance\Payments\PaymentChannels\PaymentChannelSpecification.cs | Create | | Implements IPaymentChannelSpecification |
| MyHomeRamen.Persistance\Payments\PaymentGateways\PaymentGatewayRepository.cs | Create | | Implements IPaymentGatewayRepository |
| MyHomeRamen.Persistance\Payments\PaymentGateways\PaymentGatewayQuery.cs | Create | | Implements IPaymentGatewayQuery |
| MyHomeRamen.Persistance\Payments\PaymentGateways\PaymentGatewaySpecification.cs | Create | | Implements IPaymentGatewaySpecification |
| MyHomeRamen.Persistance\Payments\Orders\OrderRepository.cs | Create | | Implements IOrderRepository |
| MyHomeRamen.Persistance\Payments\Orders\OrderQuery.cs | Create | | Implements IOrderQuery |
| MyHomeRamen.Persistance\Payments\Orders\OrderSpecification.cs | Create | | Implements IOrderSpecification |
| MyHomeRamen.Persistance\Payments\Users\UserRepository.cs | Create | | Implements IUserRepository |
| MyHomeRamen.Persistance\Payments\Users\UserQuery.cs | Create | | Implements IUserQuery |
| MyHomeRamen.Persistance\Payments\Users\UserSpecification.cs | Create | | Implements IUserSpecification |
| MyHomeRamen.Persistance\Payments\Roles\RoleRepository.cs | Create | | Implements IRoleRepository |
| MyHomeRamen.Persistance\Payments\Roles\RoleQuery.cs | Create | | Implements IRoleQuery |
| MyHomeRamen.Persistance\Payments\Roles\RoleSpecification.cs | Create | | Implements IRoleSpecification |
| MyHomeRamen.Persistance\Payments\Permissions\PermissionRepository.cs | Create | | Implements IPermissionRepository |
| MyHomeRamen.Persistance\Payments\Permissions\PermissionQuery.cs | Create | | Implements IPermissionQuery |
| MyHomeRamen.Persistance\Payments\Permissions\PermissionSpecification.cs | Create | | Implements IPermissionSpecification |
| MyHomeRamen.Persistance\DependencyInjection.cs | Modify | | Register IPaymentsDbContext, IPaymentsUnitOfWork, and all aggregate repository interfaces |
| MyHomeRamen.Worker.DatabaseInitializer\DbInitializerJob.cs | Modify | | Move paymentsDbContext from IBaseDbContext dictionary to IUnitOfWork dictionary |
| MyHomeRamen.Worker.MessagesHandler\Payments\PaymentsUserRegisteredHandler.cs | Modify | | Use dbContext.User.Exists and dbContext.Role.Query().GetByNameWithPermissions |
| MyHomeRamen.Features\Payments\Features\PaymentMethods\GetAvailableMethods\GetAvailableMethodsHandler.cs | Modify | | Use dbContext.PaymentMethod.Query().GetAvailableMethodsAsync |
| MyHomeRamen.Features\Payments\Services\PaymentService.cs | Modify | | Use dbContext.PaymentMethod.Query().GetByIdAsync |
| MyHomeRamen.Domain\Payments\Database\IPaymentsDbContext.cs | Delete | | Replaced by Features abstraction |
| MyHomeRamen.Features\Payments\Features\Common\PaymentMethodDbExtensions.cs | Delete | | Logic moved to PaymentMethodQuery |

## 3. Domain changes
- Delete `MyHomeRamen.Domain.Payments.Database.IPaymentsDbContext`.
- No entity changes.
- Migration needed: no.

## 4. Persistance extensions
- `PaymentMethodQuery.GetAvailableMethodsAsync` returns projected `List<GetAvailableMethodsResponse>`.
- `PaymentMethodQuery.GetByIdAsync` returns `PaymentMethod?` with channels (no tracking).
- `UserQuery.ExistsAsync(UserId)` returns bool using `AsNoTracking`.
- `RoleQuery.GetByNameWithPermissionsAsync(string)` returns `Role?` with permissions (no tracking).
- All other aggregates expose standard `ByIdAsync` on Query/Specification and repository CRUD via `IRepository<TEntity, TId>`.

## 5. API details
- `IPaymentsDbContext` exposes properties: `IPaymentMethodRepository PaymentMethod`, `IPaymentChannelRepository PaymentChannel`, `IPaymentGatewayRepository PaymentGateway`, `IOrderRepository Order`, `IUserRepository User`, `IRoleRepository Role`, `IPermissionRepository Permission`.
- `GetAvailableMethodsHandler` calls `dbContext.PaymentMethod.Query().GetAvailableMethodsAsync(cancellationToken)` and maps to response.
- `PaymentService.ValidatePaymentSelectionAsync` calls `dbContext.PaymentMethod.Query().GetByIdAsync(new PaymentMethodId(methodId), ct)`.
- `PaymentsUserRegisteredHandler` checks existence via `dbContext.User.Exists(userId, ct)`, loads role via `dbContext.Role.Query().GetByNameWithPermissionsAsync(roleName, ct)`, then `dbContext.User.Add(user)` and `dbContext.SaveChangesAsync`.

## 6. Tests
- Unit tests: none affected (no domain logic changes).
- Integration tests: `MyHomeRamen.IntegrationTests.PaymentsModule.InitiatePaymentTests` is empty; no updates required for this refactor.
- Worker tests: none exist; verify `PaymentsUserRegisteredHandler` still creates user when event arrives.

## 7. Risks / decisions for human approval
- `PaymentMethodDbExtensions.GetById` currently returns `IQueryable<PaymentMethod>` and is chained with `.FirstOrDefaultAsync`. Refactor returns `PaymentMethod?` directly. Confirm this is acceptable for `PaymentService`.
- `Order`, `PaymentChannel`, `PaymentGateway` aggregates have no current feature usage but still get full repository/query/specification scaffolding to satisfy `IPaymentsDbContext` contract.

## 8. Out of scope
- Adding new payment features or changing payment business rules.
- Refactoring Menu, Orders, or other modules.
- Blazor frontend changes.
