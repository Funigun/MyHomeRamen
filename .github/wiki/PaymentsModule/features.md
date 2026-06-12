# Patterns

This file contains all information about Payments module features and behaviors.
Treat it as the source of truth in case of what currently exists in the project.

---

## Domain Entities and Value Objects
Note: entity IDs are skipped on purpose since they are forced by design. Entity existence is equal to EntityId existence.

### Entities

| Entity | File path | Description |
|---|---|---|
| `PaymentMethod` | `MyHomeRamen.Domain/Payments/PaymentMethods/PaymentMethod.cs` | Payment option exposed to the customer. Holds `Name`, `ImageUrl`, `IsActive`, `DisplayOrder`, and linked `PaymentChannel` entries. |
| `PaymentChannel` | `MyHomeRamen.Domain/Payments/PaymentChannels/PaymentChannel.cs` | Payment channel entry tied to a `PaymentGateway`. Supports activation and display ordering. |
| `PaymentGateway` | `MyHomeRamen.Domain/Payments/PaymentGateways/PaymentGateway.cs` | Gateway/provider descriptor used by payment channels. |
| `Order` | `MyHomeRamen.Domain/Payments/Orders/Order.cs` | Order projection for payment flows. Stores `OriginalId` and `Amount`. |
| `User` | `MyHomeRamen.Domain/Payments/Users/User.cs` | Read-side user projection. Holds `Roles` and `Permissions`. |
| `Role` | `MyHomeRamen.Domain/Payments/Users/Role.cs` | Seeded payment role definition. |
| `Permission` | `MyHomeRamen.Domain/Payments/Users/Permission.cs` | Seeded payment permission definition. |

### Value Objects / Enums

| Type | File path | Description |
|---|---|---|


### Entity Methods (factory & mutation)

| Entity | Method | Description |
|---|---|---|
| `PaymentMethod` | `static Create(PaymentMethodId, string name, string imageUrl, bool isActive, int displayOrder)` | Creates a payment method and validates it. |
| `PaymentChannel` | `static Create(PaymentChannelId, string name, string imageUrl, bool isActive, int displayOrder, PaymentGateway paymentGateway)` | Creates a payment channel and validates it. |
| `PaymentGateway` | `static Create(PaymentGatewayId, string name)` | Creates a payment gateway and validates it. |
| `Order` | `static Create(OrderId, OrderId originalId, decimal amount)` | Creates an order projection and validates its amount. |
| `User` | `static Create(UserId, string firstName, string lastName, string email, string phoneNumber, List<Role> roles, List<Permission> permissions)` | Creates a user projection and validates its required fields. |
| `Role` | `static CreateForSeed(RoleId, string name, List<Permission> permissions)` | Creates a seeded role with permissions. |
| `Role` | `static CreateCustomerRole(RoleId, List<Permission> permissions)` | Creates the customer role variant. |
| `Permission` | `static CreateForSeed(PermissionId, string name)` | Creates a seeded permission. |
| `Permission` | `static Create(PermissionId, string name, string description)` | Creates a permission definition. |

---

## Persistence Extension methods

All extensions live in `MyHomeRamen.Persistance/Payments/Extensions/` as `partial class DbExtensions` under the `MyHomeRamen.Persistance.Common` namespace and use the C# 14 `extension` block syntax.

| File | Method | Description |
|---|---|---|
|PaymentsDbExtensions.cs|`GetAvailableMethodsQuery(this DbSet<PaymentMethod> paymentMethods)`|Filters active methods, includes active channels ordered by `DisplayOrder`, orders methods by `DisplayOrder`. Returns `IQueryable<PaymentMethod>`.|

---

## API Features

All slices are under `MyHomeRamen.Api/Payments/Features/`. Route prefix: `api/payments/`.

| Slice | Method & Route | Auth | Handler behavior | Produced Event |
|---|---|---|---|---|
| GetAvailableMethods | `GET methods/available` | AllowAnonymous | Calls `GetAvailableMethodsQuery()`, projects to `IEnumerable<PaymentMethodResponse>` via `Mappings` | N/A |