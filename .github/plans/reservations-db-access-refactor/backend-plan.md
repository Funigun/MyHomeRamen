# Plan: Reservations - Database Access Refactor

## 1. Problem
Reservations module still uses legacy `IReservationsDbContext` in Domain exposing `DbSet<T>` and inheriting `IBaseDbContext` transaction methods. Need to align with Menu refactor: move context abstraction to Features, implement repository/query/specification pattern, remove direct `DbSet` usage, and drop transaction methods.

## 2. Files to create / modify
| Path | Action | Type | Notes |
|------|--------|------|-------|
| MyHomeRamen.Features\Reservations\Features\Abstractions\IReservationsUnitOfWork.cs | Create | | Inherits IUnitOfWork |
| MyHomeRamen.Features\Reservations\Features\Abstractions\IReservationsDbContext.cs | Create | | Exposes repository properties, replaces Domain interface |
| MyHomeRamen.Features\Reservations\Features\Bookings\Common\IBookingRepository.cs | Create | | Extends IRepository<Booking, BookingId> |
| MyHomeRamen.Features\Reservations\Features\Bookings\Common\IBookingQuery.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Reservations\Features\Bookings\Common\IBookingSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Reservations\Features\Tables\Common\ITableRepository.cs | Create | | Extends IRepository<Table, TableId> |
| MyHomeRamen.Features\Reservations\Features\Tables\Common\ITableQuery.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Reservations\Features\Tables\Common\ITableSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Reservations\Features\Users\Common\IUserRepository.cs | Create | | Extends IRepository<User, UserId> |
| MyHomeRamen.Features\Reservations\Features\Users\Common\IUserQuery.cs | Create | | ExistsAsync(UserId) |
| MyHomeRamen.Features\Reservations\Features\Users\Common\IUserSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Reservations\Features\Roles\Common\IRoleRepository.cs | Create | | Extends IRepository<Role, RoleId> |
| MyHomeRamen.Features\Reservations\Features\Roles\Common\IRoleQuery.cs | Create | | GetByNameWithPermissionsAsync |
| MyHomeRamen.Features\Reservations\Features\Roles\Common\IRoleSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Reservations\Features\Permissions\Common\IPermissionRepository.cs | Create | | Extends IRepository<Permission, PermissionId> |
| MyHomeRamen.Features\Reservations\Features\Permissions\Common\IPermissionQuery.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Reservations\Features\Permissions\Common\IPermissionSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Persistance\Reservations\ReservationsDbContext.cs | Modify | | remove Begin/Commit/RollbackTransaction, implement IReservationsDbContext, add repository properties |
| MyHomeRamen.Persistance\Reservations\Bookings\BookingRepository.cs | Create | | Implements IBookingRepository |
| MyHomeRamen.Persistance\Reservations\Bookings\BookingQuery.cs | Create | | Implements IBookingQuery |
| MyHomeRamen.Persistance\Reservations\Bookings\BookingSpecification.cs | Create | | Implements IBookingSpecification |
| MyHomeRamen.Persistance\Reservations\Tables\TableRepository.cs | Create | | Implements ITableRepository |
| MyHomeRamen.Persistance\Reservations\Tables\TableQuery.cs | Create | | Implements ITableQuery |
| MyHomeRamen.Persistance\Reservations\Tables\TableSpecification.cs | Create | | Implements ITableSpecification |
| MyHomeRamen.Persistance\Reservations\Users\UserRepository.cs | Create | | Implements IUserRepository |
| MyHomeRamen.Persistance\Reservations\Users\UserQuery.cs | Create | | Implements IUserQuery |
| MyHomeRamen.Persistance\Reservations\Users\UserSpecification.cs | Create | | Implements IUserSpecification |
| MyHomeRamen.Persistance\Reservations\Roles\RoleRepository.cs | Create | | Implements IRoleRepository |
| MyHomeRamen.Persistance\Reservations\Roles\RoleQuery.cs | Create | | Implements IRoleQuery |
| MyHomeRamen.Persistance\Reservations\Roles\RoleSpecification.cs | Create | | Implements IRoleSpecification |
| MyHomeRamen.Persistance\Reservations\Permissions\PermissionRepository.cs | Create | | Implements IPermissionRepository |
| MyHomeRamen.Persistance\Reservations\Permissions\PermissionQuery.cs | Create | | Implements IPermissionQuery |
| MyHomeRamen.Persistance\Reservations\Permissions\PermissionSpecification.cs | Create | | Implements IPermissionSpecification |
| MyHomeRamen.Persistance\DependencyInjection.cs | Modify | | Register IReservationsDbContext, IReservationsUnitOfWork, and all aggregate repository interfaces |
| MyHomeRamen.Worker.DatabaseInitializer\DbInitializerJob.cs | Modify | | Move reservationsDbContext from IBaseDbContext dictionary to IUnitOfWork dictionary |
| MyHomeRamen.Worker.MessagesHandler\Reservations\ReservationsUserRegisteredHandler.cs | Modify | | Use dbContext.User.Exists and dbContext.Role.Query().GetByNameWithPermissions |
| MyHomeRamen.Domain\Reservations\Database\IReservationsDbContext.cs | Delete | | Replaced by Features abstraction |

For each of repositories run `RepositoryScaffoldScript.cs`:
> ```
> cd "C:\Users\stepn\source\repos\MyHomeRamen" && dotnet run ./Scripts/RepositoryScaffold/RepoisitoryScaffoldScript.cs -- C:\Users\stepn\source\repos\MyHomeRamen {ModuleName} {AggregatePath} {AggretageName}
> ```

Example: dotnet run ./Scripts/RepositoryScaffold/RepoisitoryScaffoldScript.cs -- C:\Users\stepn\source\repos\MyHomeRamen Reservations Tables Table

## 3. Domain changes
- Delete `MyHomeRamen.Domain.Reservations.Database.IReservationsDbContext`.
- No entity changes.
- Migration needed: no.

## 4. Persistance extensions
- `UserQuery.ExistsAsync(UserId)` returns bool using `AsNoTracking`.
- `RoleQuery.GetByNameWithPermissionsAsync(string)` returns `Role?` with permissions (no tracking).
- All other aggregates expose standard `ByIdAsync` on Query/Specification and repository CRUD via `IRepository<TEntity, TId>`.

## 5. API details
- `IReservationsDbContext` exposes properties: `IBookingRepository Booking`, `ITableRepository Table`, `IUserRepository User`, `IRoleRepository Role`, `IPermissionRepository Permission`.
- `ReservationsUserRegisteredHandler` checks existence via `dbContext.User.Exists(userId, ct)`, loads role via `dbContext.Role.Query().GetByNameWithPermissionsAsync(roleName, ct)`, then `dbContext.User.Add(user)` and `dbContext.SaveChangesAsync`.

## 6. Tests
- Unit tests: none affected.
- Integration tests: `MyHomeRamen.IntegrationTests.ReservationModule.CreateReservationTests` is empty; no updates required.
- Worker tests: none exist; verify `ReservationsUserRegisteredHandler` still creates user when event arrives.

## 7. Risks / decisions for human approval
- Reservations has no API features yet, so all aggregate repository/query/specification files are scaffolding. Confirm this is acceptable to satisfy the `IReservationsDbContext` contract.

## 8. Out of scope
- Adding new reservation features or changing reservation business rules.
- Refactoring other modules.
- Blazor frontend changes.
