# Plan: Remove `RestaurantId` from application

## Goal
Remove leftover single-tenant `RestaurantId` property/config (trial multitenancy) from domain, persistence, infrastructure, config, and tests, ahead of introducing a proper `Restaurants` module. `RestaurantConfigurationProvider`/`RestaurantConfiguration` (Name, InfrastructurePrefix, etc.) stay — only the `RestaurantId` member and its usages go.

## Execution order
1. Domain (remove property + setters)
2. Persistence (configs, query filter, DbContext seeding logic)
3. Infrastructure (`ICurrentUser`, `CacheService`)
4. Config (`RestaurantConfigurationProvider`, Blazor `RestaurantConfiguration`, AppHost)
5. appsettings files
6. Tests (unit/integration/identity)
7. EF Core migrations (drop column, one per DbContext)
8. Wiki docs
9. Build + run full test suite

---

## 1. Domain

- `MyHomeRamen.Domain\Abstractions\AuditableEntity.cs`
  Remove `RestaurantId` property and `SetRestaurantId(Guid)` method entirely.

- `MyHomeRamen.Domain\Identity\Users\User.cs`
  Remove `SetRestaurantId(Guid restaurantId)` override (calls base `AuditableEntity` member — no longer exists).

- `MyHomeRamen.Domain\Identity\Roles\Role.cs`
  Remove duplicate `public Guid RestaurantId { get; private set; }` property (it already inherits `AuditableEntity`, so this was a redundant shadow/duplicate column).

---

## 2. Persistence — configurations (remove `builder.Property(x => x.RestaurantId)...` block)

Files (one `IEntityTypeConfiguration<T>` block each):
- `Menu\Configurations\CategoryConfiguration.cs`
- `Menu\Configurations\IngredientConfiguration.cs`
- `Menu\Configurations\PermissionConfiguration.cs`
- `Menu\Configurations\ProductConfiguration.cs`
- `Menu\Configurations\RoleConfiguration.cs`
- `Menu\Configurations\UserConfiguration.cs`
- `Orders\Configurations\IngredientConfiguration.cs`
- `Orders\Configurations\OrderConfiguration.cs`
- `Orders\Configurations\PaymentConfiguration.cs`
- `Orders\Configurations\ProductConfiguration.cs`
- `Payments\Configurations\OrderConfiguration.cs`
- `Payments\Configurations\PermissionConfiguration.cs`
- `Payments\Configurations\RoleConfiguration.cs`
- `Payments\Configurations\UserConfiguration.cs`
- `Reservations\Configurations\BookingConfiguration.cs`
- `Reservations\Configurations\PermissionConfiguration.cs`
- `Reservations\Configurations\RoleConfiguration.cs`
- `Reservations\Configurations\TableConfiguration.cs`
- `Reservations\Configurations\UserConfiguration.cs`
- `ShoppingCart\Configurations\BasketConfiguration.cs`
- `ShoppingCart\Configurations\IngredientConfiguration.cs`
- `ShoppingCart\Configurations\PermissionConfiguration.cs`
- `ShoppingCart\Configurations\ProductConfiguration.cs`
- `ShoppingCart\Configurations\RoleConfiguration.cs`
- `ShoppingCart\Configurations\UserConfiguration.cs`

## 3. Persistence — DbContexts

- `Menu\MenuDbContext.cs`, `Orders\OrdersDbContext.cs`, `Payments\PaymentsDbContext.cs`, `Reservations\ReservationsDbContext.cs`, `ShoppingCart\ShoppingCartDbContext.cs`
  In `UpdateEntities()`, remove `entry.Entity.SetRestaurantId(_currentUser.RestaurantId);` line under `EntityState.Added`.

- `Identity\IdentityDbContext.cs` (bigger change, uses query filter):
  - Remove `b.HasQueryFilter(u => u.RestaurantId == _restaurantConfiguration.RestaurantId);`
  - Remove `b.Property(u => u.RestaurantId).IsRequired(true);`
  - Remove both `entry.Entity.SetRestaurantId(_restaurantConfiguration.RestaurantId);` calls in `UpdateEntities()`.
  - `_restaurantConfiguration` field/ctor params (`RestaurantConfigurationProvider configFactory`) become unused for this purpose — check remaining usages in this file; if none remain, remove the field and simplify both constructors to drop the `RestaurantConfigurationProvider` parameter (also update `IIdentityDbContext` interface / any call sites and DI registration if the ctor param is dropped).

- `Identity\IdentityDbContextFactory.cs`
  If `RestaurantConfigurationProvider` param removed from `IdentityDbContext` ctor, update the `new IdentityDbContext(...)` call here to match new signature.

---

## 4. Infrastructure / Authorization

- `MyHomeRamen.Features\Common\Authorization\ICurrentUser.cs` — remove `Guid RestaurantId { get; init; }`.
- `MyHomeRamen.Features\Common\Authorization\CurrentUser.cs` — remove `RestaurantId` property and its `configurationProvider.RestaurantId` initializer; drop `RestaurantConfigurationProvider configurationProvider` ctor param if unused afterward (verify — currently only used for `RestaurantId`).
- `MyHomeRamen.Worker.DatabaseInitializer\Config\WorkerUser.cs` — remove `RestaurantId` property (`Guid.Parse(configuration[...])`); drop unused `IConfiguration` ctor param if nothing else uses it.
- `MyHomeRamen.Worker.MessagesHandler\Common\WorkerUser.cs` — same as above.
- `MyHomeRamen.Features\Common\Cache\ICacheService.cs` — remove `RemoveByRestaurantIdAsync(CancellationToken)` method.
- `MyHomeRamen.Infrastructure\Cache\CacheService.cs`:
  - Remove `RemoveByRestaurantIdAsync` implementation.
  - Remove `restaurantId` prefixing logic in `GetOrSetAsync`, `RemoveByKeyAsync`, `RemoveByTagsAsync` (drop the `RestaurantConfigurationProvider` scope resolution + `restaurantId` string, and stop prefixing keys/tags with it). Cache keys become `{policy.Module}_{policy.Key}` etc.
  - Search codebase for callers of `RemoveByRestaurantIdAsync` and update/remove those call sites too.

---

## 5. Configuration providers

- `MyHomeRamen.Features\Common\Configurations\RestaurantConfigurationProvider.cs` — remove `RestaurantId` property only; keep `InfrastructurePrefix`, `RestaurantName`.
- `MyHomeRamen.Blazor\MyHomeRamen.Blazor\Common\Configuration\RestaurantConfiguration.cs` — remove `RestaurantId` property only; keep the rest (Name, LayoutType, Tagline, etc.).

## 6. AppHost

- `MyHomeRamen.AppHost\Configurations\Common\ConfigurationExtensions.cs`
  In `WithRestaurantConfig`, remove `restaurantId` read + `WithEnvironment($"...__RestaurantId", restaurantId)` line; keep Name/InfrastructurePrefix propagation.
- No change needed to `ConfigurationConstants.cs` (`RestaurantConfigurationSection` constant stays — section still used for Name/InfrastructurePrefix).

## 7. appsettings files

- `MyHomeRamen.Api\appsettings.Test.json` — remove `"RestaurantId": "..."` line from `RestaurantConfiguration` section.
- `MyHomeRamen.AppHost\appsettings.Development.json` — remove `"RestaurantId": "..."` line from `RestaurantConfiguration` section.
- Search for any other environment-specific appsettings (`appsettings.json`, `appsettings.Production.json`, user secrets templates) under `MyHomeRamen.Api`, `MyHomeRamen.Blazor*`, `MyHomeRamen.Worker.*`, `MyHomeRamen.AppHost` for a `RestaurantConfiguration:RestaurantId` key and remove it there too (only the two files above matched in current search, but re-check user secrets since those aren't in source control).

---

## 8. Tests

- `MyHomeRamen.IntegrationTests\Authentication\FakeUser.cs` — remove `RestaurantId` property (implements `ICurrentUser`).
- `MyHomeRamen.IdentityApi.IntegrationTests\Common\IdentityFakeUser.cs`:
  - Remove `RestaurantId` property from `IdentityFakeUser`.
  - Remove `["RestaurantConfiguration:RestaurantId"] = dataSeeder.SeededRestaurantId.ToString(),` from `IdentityFakeRestaurantConfig.Create`.
- `MyHomeRamen.IdentityApi.IntegrationTests\Common\IdentityWebApiFactory.cs` — remove `builder.UseSetting("RestaurantConfiguration:RestaurantId", DataSeeder.SeededRestaurantId.ToString());`.
- `MyHomeRamen.IdentityApi.IntegrationTests\Common\Data\DataSeeder.cs` — remove `SeededRestaurantId` field (verify no other usages left after above changes).
- `MyHomeRamen.UnitTests\OrdersModule\Orders\OrderValidationTests.cs` — remove unused `DefaultRestaurantId` field (verify unused first — currently declared but not referenced elsewhere in file besides declaration).
- `MyHomeRamen.UnitTests\ReservationsModule\Bookings\BookingValidationTests.cs` — remove unused `DefaultRestaurantId` field.
- `MyHomeRamen.UnitTests\ReservationsModule\Users\UserValidationTests.cs` — remove unused `TestRestaurantId` field.
- After all production changes, run full build + rerun Architecture/Unit/Integration/System test suites (see `.github\instructions\backend-tests.instructions.md`) to confirm nothing else depends on `RestaurantId` (e.g. `ICurrentUser` implementations added later, seeding helpers not caught by grep).

---

## 9. EF Core migrations (drop column `RestaurantId`)

For every DbContext with the column, add a new migration to drop it (do **not** hand-edit old migrations/snapshots):
- `MyHomeRamen.Persistance\Identity` (`IdentityDbContext`) — drops from `Users`, `Roles`.
- `MyHomeRamen.Persistance\Menu` (`MenuDbContext`) — drops from Product/Category/Ingredient/User/Role/Permission tables.
- `MyHomeRamen.Persistance\Orders` (`OrdersDbContext`) — drops from Order/Payment/Product/Ingredient tables.
- `MyHomeRamen.Persistance\Payments` (`PaymentsDbContext`) — drops from Order/User/Role/Permission tables.
- `MyHomeRamen.Persistance\Reservations` (`ReservationsDbContext`) — drops from Booking/Table/User/Role/Permission tables.
- `MyHomeRamen.Persistance\ShoppingCart` (`ShoppingCartDbContext`) — drops from Basket/Product/Ingredient/User/Role/Permission tables.

Command pattern per module (run from repo root, after code changes above compile):
```
dotnet ef migrations add Remove_RestaurantId --project MyHomeRamen.Persistance --startup-project MyHomeRamen.Api --context <ModuleName>DbContext --output-dir <Module>/Migrations
```
This regenerates the model snapshot per module and produces the `DropColumn` migration automatically — do not manually edit `*ModelSnapshot.cs` files.

---

## 10. Documentation

- `Wiki\Architecture\0004-restaurant-configuration.md` — update to reflect `RestaurantId` removal from `RestaurantConfigurationProvider` (still valid for Name/InfrastructurePrefix); note multitenancy trial superseded by upcoming `Restaurants` module.
- `Wiki\Architecture\0005-repository-strategy.md` — check/remove `RestaurantId` mentions (e.g. filtering behavior in repositories).
- `Wiki\Getting Started\0001-restaurant-configuration.md` — remove `RestaurantId` setup step from onboarding doc.
- `.github\wiki\architecture.md` — remove/adjust `RestaurantId` mentions.

---

## Notes / risks
- `IdentityDbContext` currently is the **only** place using `HasQueryFilter` on `RestaurantId` — removing it changes query behavior for `Users`/`Roles` (previously implicitly filtered per-restaurant; now unfiltered, which is expected/desired per this cleanup since app is single-tenant until `Restaurants` module lands).
- Dropping `RestaurantId` columns is a breaking schema change — coordinate with any existing non-dev databases; existing local dev DBs need migration applied (`dotnet ef database update` per module) or `Worker.DatabaseInitializer` re-run.
- Double check no leftover references after code changes via `grep -r RestaurantId` across the whole repo before considering this complete — some migrations/snapshots will still reference it historically and are expected to remain (audit trail), only the *new* migration + all runtime code should be free of it.
