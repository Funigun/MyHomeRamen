
Database access refactoring:


# Current State:

IBaseDbContext in domain: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Domain\Abstractions\IBaseDbContext.cs

## Concrete variations in Domain:

IMenuDbContext in domain: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Domain\Menu\Database\IMenuDbContext.cs
IOrderDbContext in domain: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Domain\Order\Database\IOrderDbContext.cs
IPaymentsDbContext in domain: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Domain\Payments\Database\IPaymentDbContext.cs
IShoppingCartDbContext in domain: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Domain\ShoppingCart\Database\IShoppingCartDbContext.cs
IUsersDbContext in domain: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Domain\Users\Database\IUsersDbContext.cs
IReservationsDbContext in domain: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Domain\Reservations\Database\IReservationsDbContext.cs


## Implementation (based on Menu module):

MenuDbContext in persistance: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Domain\Menu\Database\IMenuDbContext.cs

## Concrete queries in Features layer (based on Menu module):

Generic repository extensions: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Features\Common\Repository\RepositoryExtensions.cs
Aggregate queries: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Features\Menu\Features\Categories\Common\CategoryDbExtensions.cs


# Issues: 
- Domain and Feature layers are coupled to EF Core and DbContext
- It is hard to add caching, as extension methods would need additional parameters that could be handled via DI


# Solution - example based on Menu module:

IUnitOfWork in features: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Features\Common\Repository\IUnitOfWork.cs
IRepository in features: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Features\Common\Repository\IRepository.cs

IMenuUnitOfWork in features: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Features\Menu\Features\Abstractions\IMenuUnitOfWork.cs
IMenuUnitOfWork implements IUnitOfWork

ICategoryQuery in features: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Features\Menu\Features\Categories\Common\ICategoryQuery.cs
ICategoryQuery contains query methods that use AsNoTracking - each method here must

ICategorySpecification in features: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Features\Menu\Features\Categories\Common\ICategorySpecification.cs
ICategorySpecification contains methods for persistance operations without using AsNoTracking

ICategoryRepository in features: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Features\Menu\Features\Categories\Common\ICategoryRepository.cs

ICategoryRepository implements both ICategoryQuery and ICategorySpecification and exposes methods to return ICategoryQuery and ICategorySpecification


Concrete implementation:

MenuDbContext becomes partial class that implements IMenuUnitOfWork, ICategoryRepository, ICategoryQuery and ICategorySpecification with following split:

IMenuUnitOfWork: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Persistance\Menu\MenuDbContext.cs
ICategoryRepository: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Persistance\Menu\Categories\CategoryRepository.cs
ICategoryQuery: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Persistance\Menu\Categories\CategoryQuery.cs
ICategorySpecification: C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Persistance\Menu\Categories\CategorySpecification.cs

Dependency injection (C:\Users\stepn\source\repos\MyHomeRamen\MyHomeRamen.Persistance\DependencyInjection.cs):

- services.AddScoped<IMenuUnitOfWork>(provider => provider.GetRequiredService<MenuDbContext>());
- services.AddScoped<ICategoryRepository>(provider => provider.GetRequiredService<MenuDbContext>());


Task: Prepare refactoring plan for each module (Menu (missing aggregates), Order, Payments, ShoppingCart, Users, Reservations) based on the Menu module example.

Each module should have its own file with plan.
Cleanupt stage should be in separate plan file.

Steps to take:
- define unit of work: I{Module}UnitOfWork
- define for each aggregate:I{Aggregate}Repository, I{Aggregate}Query, I{Aggregate}Specification)
- define implementations following partial class pattern
- move extension methods to I{Aggregate}Query and I{Aggregate}Specification
- update features in Features project
- update integration tests to use new interfaces and implementations
- update workers to use new interfaces and implementations


Once all modules migrated:
- remove old abstractions
- remove references to EF Core from Domain and Features layers