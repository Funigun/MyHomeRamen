# Plan: Users - Database Access Refactor

## 1. Problem
Users module still uses legacy `IUsersDbContext` in Domain exposing `DbSet<T>` and inheriting `IBaseDbContext` transaction methods. Need to align with Menu refactor: move context abstraction to Features, implement repository/query/specification pattern, remove direct `DbSet` usage, and drop transaction methods. `UsersDbContext` inherits `IdentityDbContext`, so internal `Users`/`Roles` DbSets remain but are no longer exposed on the interface.

## 2. Files to create / modify
| Path | Action | Type | Notes |
|------|--------|------|-------|
| MyHomeRamen.Features\Users\Features\Abstractions\IUsersUnitOfWork.cs | Create | | Inherits IUnitOfWork |
| MyHomeRamen.Features\Users\Features\Abstractions\IUsersDbContext.cs | Create | | Exposes repository properties, replaces Domain interface |
| MyHomeRamen.Features\Users\Features\Users\Common\IUserRepository.cs | Create | | Extends IRepository<User, Guid> |
| MyHomeRamen.Features\Users\Features\Users\Common\IUserQuery.cs | Create | | GetByKeycloakIdAsync, GetIdByKeycloakIdAsync, GetByIdAsync, GetNumberOfAddressesAsync, AddressExistsAsync |
| MyHomeRamen.Features\Users\Features\Users\Common\IUserSpecification.cs | Create | | GetByIdAsync, GetByKeycloakIdAsync (tracked, includes Addresses) |
| MyHomeRamen.Features\Users\Features\Roles\Common\IRoleRepository.cs | Create | | Extends IRepository<Role, Guid> |
| MyHomeRamen.Features\Users\Features\Roles\Common\IRoleQuery.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Users\Features\Roles\Common\IRoleSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Users\Features\Addresses\Common\IAddressRepository.cs | Create | | Extends IRepository<Address, Guid> |
| MyHomeRamen.Features\Users\Features\Addresses\Common\IAddressQuery.cs | Create | | ByIdAsync |
| MyHomeRamen.Features\Users\Features\Addresses\Common\IAddressSpecification.cs | Create | | ByIdAsync |
| MyHomeRamen.Persistance\Users\UsersDbContext.cs | Modify | | Make partial, remove Begin/Commit/RollbackTransaction, implement IUsersDbContext, add repository properties |
| MyHomeRamen.Persistance\Users\Users\UserRepository.cs | Create | | Implements IUserRepository |
| MyHomeRamen.Persistance\Users\Users\UserQuery.cs | Create | | Implements IUserQuery |
| MyHomeRamen.Persistance\Users\Users\UserSpecification.cs | Create | | Implements IUserSpecification |
| MyHomeRamen.Persistance\Users\Roles\RoleRepository.cs | Create | | Implements IRoleRepository |
| MyHomeRamen.Persistance\Users\Roles\RoleQuery.cs | Create | | Implements IRoleQuery |
| MyHomeRamen.Persistance\Users\Roles\RoleSpecification.cs | Create | | Implements IRoleSpecification |
| MyHomeRamen.Persistance\Users\Addresses\AddressRepository.cs | Create | | Implements IAddressRepository |
| MyHomeRamen.Persistance\Users\Addresses\AddressQuery.cs | Create | | Implements IAddressQuery |
| MyHomeRamen.Persistance\Users\Addresses\AddressSpecification.cs | Create | | Implements IAddressSpecification |
| MyHomeRamen.Persistance\DependencyInjection.cs | Modify | | Register IUsersDbContext, IUsersUnitOfWork, and all aggregate repository interfaces |
| MyHomeRamen.Worker.DatabaseInitializer\DbInitializerJob.cs | Modify | | Move userContext from IBaseDbContext dictionary to IUnitOfWork dictionary |
| MyHomeRamen.Features\Users\Features\Account\CreateAddress\CreateAddressHandler.cs | Modify | | Use dbContext.User.Specification().GetByIdAsync and dbContext.Address.Add |
| MyHomeRamen.Features\Users\Features\Account\CreateAddress\CreateAddressValidator.cs | Modify | | Use dbContext.User.Query().GetNumberOfAddressesAsync |
| MyHomeRamen.Features\Users\Features\Account\DeleteAddress\DeleteAddressHandler.cs | Modify | | Use dbContext.User.Specification().GetByIdAsync |
| MyHomeRamen.Features\Users\Features\Account\DeleteAddress\DeleteAddressValidator.cs | Modify | | Use dbContext.User.Query().AddressExistsAsync |
| MyHomeRamen.Features\Users\Features\Account\GetAddresses\GetAddressesHandler.cs | Modify | | Use dbContext.User.Query().GetByIdAsync |
| MyHomeRamen.Features\Users\Features\Account\GetDetails\GetDetailsHandler.cs | Modify | | Use dbContext.User.Query().GetByIdAsync |
| MyHomeRamen.Features\Users\Features\Account\GetId\GetMyIdHandler.cs | Modify | | Use dbContext.User.Query().GetIdByKeycloakIdAsync |
| MyHomeRamen.Features\Users\Features\Account\Register\RegisterHandler.cs | Modify | | Use dbContext.User.Add |
| MyHomeRamen.Features\Users\Features\Account\RegisterGuest\RegisterGuestHandler.cs | Modify | | Use dbContext.User.Query().GetGuestIdByGuestIdAsync and dbContext.User.Add |
| MyHomeRamen.Features\Users\Features\Account\UpdateAddress\UpdateAddressHandler.cs | Modify | | Use dbContext.User.Specification().GetByKeycloakIdAsync |
| MyHomeRamen.Features\Users\Features\Employees\RegisterEmployee\RegisterEmployeeHandler.cs | Modify | | Use dbContext.User.Add |
| MyHomeRamen.IdentityApi.IntegrationTests\Common\IdentityWebApiFactory.cs | Modify | | Expose IUsersDbContext instead of UsersDbContext |
| MyHomeRamen.IdentityApi.IntegrationTests\Common\Data\DataSeeder.cs | Modify | | Use dbContext.User.Add and dbContext.Address.Add |
| MyHomeRamen.IdentityApi.IntegrationTests\IdentityModule\Addresses\GetAddressesTests.cs | Modify | | Use dbContext.User.Add |
| MyHomeRamen.Domain\Users\Database\IUsersDbContext.cs | Delete | | Replaced by Features abstraction |
| MyHomeRamen.Features\Users\Extensions\AddressDbExtensions.cs | Delete | | Logic moved to UserQuery/UserSpecification |
| MyHomeRamen.Features\Users\Extensions\UserDbExtensions.cs | Delete | | Logic moved to UserQuery/UserSpecification |

## 3. Domain changes
- Delete `MyHomeRamen.Domain.Users.Database.IUsersDbContext`.
- No entity changes.
- Migration needed: no.

## 4. Persistance extensions
- `UserQuery.GetByKeycloakIdAsync(string, ct)` returns `User?` (no tracking).
- `UserQuery.GetIdByKeycloakIdAsync(string, ct)` returns `Guid?`.
- `UserQuery.GetByIdAsync(Guid, ct)` returns `User?` with addresses (no tracking).
- `UserQuery.GetNumberOfAddressesAsync(Guid, ct)` returns int.
- `UserQuery.AddressExistsAsync(Guid userId, Guid addressId, ct)` returns bool.
- `UserSpecification.GetByIdAsync(Guid, ct)` returns `User` tracked with addresses.
- `UserSpecification.GetByKeycloakIdAsync(string, ct)` returns `User?` tracked with addresses.
- `AddressRepository.Add(Address)` used by `CreateAddressHandler` and test seeder.
- `Role` and `Address` expose standard `ByIdAsync` on Query/Specification and repository CRUD.

## 5. API details
- `IUsersDbContext` exposes properties: `IUserRepository User`, `IRoleRepository Role`, `IAddressRepository Address`.
- `CreateAddressHandler` loads user via `dbContext.User.Specification().GetByIdAsync(currentUser.UserId, ct)`, calls `user.AddAddress(address)`, then `dbContext.Address.Add(address)` and `dbContext.SaveChangesAsync`.
- `DeleteAddressHandler` loads user via `dbContext.User.Specification().GetByIdAsync(currentUser.UserId, ct)`, calls `user.RemoveAddress(command.Id)`, then `dbContext.SaveChangesAsync`.
- `UpdateAddressHandler` loads user via `dbContext.User.Specification().GetByKeycloakIdAsync(currentUser.Id, ct)`, calls `user.UpdateAddress(...)`, then `dbContext.SaveChangesAsync`.
- `GetAddressesHandler`, `GetDetailsHandler` use `dbContext.User.Query().GetByIdAsync(...)`.
- `GetMyIdHandler` uses `dbContext.User.Query().GetIdByKeycloakIdAsync(...)`.
- `RegisterGuestHandler` checks existing guest via `dbContext.User.Query().GetGuestIdByGuestIdAsync(...)`.
- `RegisterHandler` and `RegisterEmployeeHandler` use `dbContext.User.Add(user)`.

## 6. Tests
- Unit tests: none affected (no domain logic changes).
- Integration tests: update `IdentityWebApiFactory`, `DataSeeder`, and `GetAddressesTests` to use `IUsersDbContext` and repository methods instead of direct `DbSet` access.
- Verify all Users module integration tests still pass after replacing `UsersDbContext.Users.Add` with `dbContext.User.Add`.

## 7. Risks / decisions for human approval
- `UsersDbContext` inherits `IdentityDbContext<User, Role, Guid>`. The internal `Users` and `Roles` DbSets from Identity will remain but must not be exposed on `IUsersDbContext`. Confirm repository implementations can use the inherited DbSets internally.
- `Address` aggregate currently has no standalone query/spec usage except `Add`. Still create full scaffolding for consistency.

## 8. Out of scope
- Changing Identity schema or ASP.NET Identity configuration.
- Refactoring other modules.
- Blazor frontend changes.
