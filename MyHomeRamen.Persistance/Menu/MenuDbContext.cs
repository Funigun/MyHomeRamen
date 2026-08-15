using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Permssions;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Domain.Menu.Roles;
using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;
using MyHomeRamen.Features.Menu.Features.Permissions.Common;
using MyHomeRamen.Features.Menu.Features.Products.Common;
using MyHomeRamen.Features.Menu.Features.Roles;
using MyHomeRamen.Features.Menu.Features.Users.Common;
using MyHomeRamen.Persistance.Menu.Converters;

namespace MyHomeRamen.Persistance.Menu;

public sealed class MenuDbContext(DbContextOptions<MenuDbContext> options) : DbContext(options), IMenuDbContext
{
    private readonly ICurrentUser _currentUser = default!;
    private readonly IServiceProvider _serviceProvider = default!;

    public MenuDbContext(DbContextOptions<MenuDbContext> options, ICurrentUser currentUser) : this(options)
    {
        _currentUser = currentUser;
    }

    public MenuDbContext(DbContextOptions<MenuDbContext> options, ICurrentUser currentUser, IServiceProvider serviceProvider) : this(options)
    {
        _currentUser = currentUser;
        _serviceProvider = serviceProvider;
    }

    public DbSet<Product> Products { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Ingredient> Ingredients { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Permission> Permissions { get; set; }

    public IProductRepository Product => _serviceProvider.GetService<IProductRepository>() ?? throw new InvalidOperationException("ProductRepository is not registered in the service provider.");

    public ICategoryRepository Category => _serviceProvider.GetService<ICategoryRepository>() ?? throw new InvalidOperationException("CategoryRepository is not registered in the service provider.");

    public IIngredientRepository Ingredient => _serviceProvider.GetService<IIngredientRepository>() ?? throw new InvalidOperationException("IngredientRepository is not registered in the service provider.");

    public IUserRepository User => _serviceProvider.GetService<IUserRepository>() ?? throw new InvalidOperationException("UserRepository is not registered in the service provider.");

    public IRoleRepository Role => _serviceProvider.GetService<IRoleRepository>() ?? throw new InvalidOperationException("RoleRepository is not registered in the service provider.");

    public IPermissionRepository Permission => _serviceProvider.GetService<IPermissionRepository>() ?? throw new InvalidOperationException("PermissionRepository is not registered in the service provider.");

    public async Task<bool> EnsureCreated(CancellationToken cancellationToken)
    {
        IRelationalDatabaseCreator? dbCreator = Microsoft.EntityFrameworkCore.Infrastructure.AccessorExtensions.GetService<IRelationalDatabaseCreator>(Database);

        bool dbExists = dbCreator != null && await dbCreator.ExistsAsync(cancellationToken);

        if (!dbExists)
        {
            await dbCreator!.CreateAsync(cancellationToken);
        }

        return dbExists;
    }

    public async Task<int> ExecuteSql(FormattableString sql, CancellationToken cancellationToken)
    {
        return await Database.ExecuteSqlInterpolatedAsync(sql, cancellationToken);
    }

    public async Task Migrate(CancellationToken cancellationToken)
    {
        if ((await Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            await Database.MigrateAsync(cancellationToken);
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        UpdateEntities();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task Seed(CancellationToken cancellationToken)
    {
        IEnumerable<string> roles = RoleConstants.AvailableRoles;
        IEnumerable<string> permissions = PermissionConstants.AvailablePermissions;

        HashSet<Permission> existingPermissions = await Permissions.ToHashSetAsync(cancellationToken);

        IEnumerable<Permission> permissionsToAdd = permissions.Except(existingPermissions.Select(p => p.Name))
                                                              .Select(permission => Domain.Menu.Permssions.Permission.CreateForSeed(new PermissionId(Guid.NewGuid()), permission))
                                                              .ToList();

        if (permissionsToAdd.Any())
        {
            await Permissions.AddRangeAsync(permissionsToAdd, cancellationToken);
            await SaveChangesAsync(cancellationToken);
        }

        existingPermissions = await Permissions.ToHashSetAsync(cancellationToken);
        HashSet<string> existingRoles = await Roles.AsNoTracking().Select(role => role.Name).ToHashSetAsync(cancellationToken);
        IEnumerable<Role> rolesToAdd = roles.Except(existingRoles)
                                            .Select(role => Domain.Menu.Roles.Role.CreateForSeed
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("menu");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MenuDbContext).Assembly, type => type.Namespace != null && type.Namespace.Contains("Menu.Configurations", StringComparison.Ordinal));
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<ProductId>().HaveConversion<ProductIdConverter>();
        configurationBuilder.Properties<CategoryId>().HaveConversion<CategoryIdConverter>();
        configurationBuilder.Properties<IngredientId>().HaveConversion<IngredientIdConverter>();
        configurationBuilder.Properties<UserId>().HaveConversion<UserIdConverter>();
        configurationBuilder.Properties<RoleId>().HaveConversion<RoleIdConverter>();
        configurationBuilder.Properties<PermissionId>().HaveConversion<PermissionIdConverter>();
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
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedBy = _currentUser.Id.ToString();
                    entry.Entity.ModifiedOn = currentDateTime;
                    break;
            }
        }
    }
}
