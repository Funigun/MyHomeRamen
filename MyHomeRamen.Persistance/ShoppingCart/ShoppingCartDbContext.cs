using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Domain.ShoppingCart.PaymentDetails;
using MyHomeRamen.Domain.ShoppingCart.Permissions;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Roles;
using MyHomeRamen.Domain.ShoppingCart.ShippingDetails;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.BasketItems.Common;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;
using MyHomeRamen.Features.ShoppingCart.Features.Ingredients.Common;
using MyHomeRamen.Features.ShoppingCart.Features.Permissions.Common;
using MyHomeRamen.Features.ShoppingCart.Features.Products.Common;
using MyHomeRamen.Features.ShoppingCart.Features.Roles.Common;
using MyHomeRamen.Features.ShoppingCart.Features.Users.Common;
using MyHomeRamen.Persistance.ShoppingCart.Converters;

namespace MyHomeRamen.Persistance.ShoppingCart;

public sealed partial class ShoppingCartDbContext(DbContextOptions<ShoppingCartDbContext> options) : DbContext(options), IShoppingCartDbContext
{
    private readonly ICurrentUser _currentUser = default!;
    private readonly IServiceProvider _serviceProvider = default!;

    public ShoppingCartDbContext(DbContextOptions<ShoppingCartDbContext> options, ICurrentUser currentUser) : this(options)
    {
        _currentUser = currentUser;
    }

    public ShoppingCartDbContext(DbContextOptions<ShoppingCartDbContext> options, ICurrentUser currentUser, IServiceProvider serviceProvider) : this(options)
    {
        _currentUser = currentUser;
        _serviceProvider = serviceProvider;
    }

    public DbSet<Basket> ShoppingCarts { get; set; }

    public DbSet<BasketItem> BasketItems { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Ingredient> Ingredients { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Permission> Permissions { get; set; }

    public DbSet<PaymentDetails> PaymentDetailEntries { get; set; }

    public DbSet<ShippingDetails> ShippingDetailEntries { get; set; }

    public IBasketRepository Basket => _serviceProvider.GetService<IBasketRepository>() ?? throw new InvalidOperationException("BasketRepository is not registered in the service provider.");

    public IBasketItemRepository BasketItem => _serviceProvider.GetService<IBasketItemRepository>() ?? throw new InvalidOperationException("BasketItemRepository is not registered in the service provider.");

    public IProductRepository Product => _serviceProvider.GetService<IProductRepository>() ?? throw new InvalidOperationException("ProductRepository is not registered in the service provider.");

    public IIngredientRepository Ingredient => _serviceProvider.GetService<IIngredientRepository>() ?? throw new InvalidOperationException("IngredientRepository is not registered in the service provider.");

    public IUserRepository User => _serviceProvider.GetService<IUserRepository>() ?? throw new InvalidOperationException("UserRepository is not registered in the service provider.");

    public IRoleRepository Role => _serviceProvider.GetService<IRoleRepository>() ?? throw new InvalidOperationException("RoleRepository is not registered in the service provider.");

    public IPermissionRepository Permission => _serviceProvider.GetService<IPermissionRepository>() ?? throw new InvalidOperationException("PermissionRepository is not registered in the service provider.");

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        UpdateEntities();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateEntities()
    {
        DateTime currentDateTime = DateTime.UtcNow;

        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _currentUser.Id.ToString();
                    entry.Entity.CreatedOn = currentDateTime;
                    entry.Entity.SetRestaurantId(_currentUser.RestaurantId);
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedBy = _currentUser.Id.ToString();
                    entry.Entity.ModifiedOn = currentDateTime;
                    break;
            }
        }
    }

    public async Task<bool> EnsureCreated(CancellationToken cancellationToken)
    {
        IRelationalDatabaseCreator? dbCreator = AccessorExtensions.GetService<IRelationalDatabaseCreator>(Database);

        bool dbExists = dbCreator != null && await dbCreator.ExistsAsync(cancellationToken);

        if (!dbExists)
        {
            await dbCreator!.CreateAsync(cancellationToken);
        }

        return dbExists;
    }

    public async Task Migrate(CancellationToken cancellationToken)
    {
        if ((await Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            await Database.MigrateAsync(cancellationToken);
        }
    }

    public async Task Seed(CancellationToken cancellationToken)
    {
        IEnumerable<string> roles = RoleConstants.AvailableRoles;
        IEnumerable<string> permissions = PermissionConstants.AvailablePermissions;

        HashSet<Permission> existingPermissions = await Permissions.ToHashSetAsync(cancellationToken);

        IEnumerable<Permission> permissionsToAdd = permissions.Except(existingPermissions.Select(p => p.Name))
                                                              .Select(permission => Domain.ShoppingCart.Permissions.Permission.CreateForSeed(new PermissionId(Guid.NewGuid()), permission))
                                                              .ToList();

        if (permissionsToAdd.Any())
        {
            await Permissions.AddRangeAsync(permissionsToAdd, cancellationToken);
            await SaveChangesAsync(cancellationToken);
        }

        existingPermissions = await Permissions.ToHashSetAsync(cancellationToken);
        HashSet<string> existingRoles = await Roles.AsNoTracking().Select(role => role.Name).ToHashSetAsync(cancellationToken);
        IEnumerable<Role> rolesToAdd = roles.Except(existingRoles)
                                            .Select(role => Domain.ShoppingCart.Roles.Role.CreateForSeed
                                                        (
                                                            new RoleId(Guid.NewGuid()),
                                                            role,
                                                            existingPermissions.Where(p => RoleConstants.DefaultPermissions[role].Contains(p.Name))
                                                                               .ToList()
                                                        )
                                                   );

        if (rolesToAdd.Any())
        {
            await Roles.AddRangeAsync(rolesToAdd, cancellationToken);
            await SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> ExecuteSql(FormattableString sql, CancellationToken cancellationToken)
    {
        return await Database.ExecuteSqlInterpolatedAsync(sql, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("basket");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShoppingCartDbContext).Assembly, type => type.Namespace != null && type.Namespace.Contains("ShoppingCart.Configurations", StringComparison.OrdinalIgnoreCase));
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<BasketId>().HaveConversion<BasketIdConverter>();
        configurationBuilder.Properties<BasketItemId>().HaveConversion<BasketItemIdConverter>();
        configurationBuilder.Properties<ProductId>().HaveConversion<ProductIdConverter>();
        configurationBuilder.Properties<IngredientId>().HaveConversion<IngredientIdConverter>();
        configurationBuilder.Properties<UserId>().HaveConversion<UserIdConverter>();
        configurationBuilder.Properties<RoleId>().HaveConversion<RoleIdConverter>();
        configurationBuilder.Properties<PermissionId>().HaveConversion<PermissionIdConverter>();
    }
}
