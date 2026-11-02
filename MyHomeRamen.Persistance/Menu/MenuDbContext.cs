using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Domain.Menu.RestaurantIngredientPrices;
using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Domain.Menu.MenuSections;
using MyHomeRamen.Domain.Menu.MenuSectionItems;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;
using MyHomeRamen.Features.Menu.Features.Products.Common;
using MyHomeRamen.Features.Menu.Features.Offers.Common;
using MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.Common;
using MyHomeRamen.Features.Menu.Features.MenuCards.Common;
using MyHomeRamen.Persistance.Menu.Converters;

namespace MyHomeRamen.Persistance.Menu;

public sealed class MenuDbContext(DbContextOptions<MenuDbContext> options) : DbContext(options), IMenuDbContext
{
    private readonly ICurrentUser _currentUser = default!;
    private readonly IServiceProvider _serviceProvider = default!;
    private ICompanyContext _companyContext = new CompanyContext();

    public MenuDbContext(DbContextOptions<MenuDbContext> options, ICurrentUser currentUser) : this(options)
    {
        _currentUser = currentUser;
    }

    public MenuDbContext(DbContextOptions<MenuDbContext> options, ICurrentUser currentUser, IServiceProvider serviceProvider) : this(options)
    {
        _currentUser = currentUser;
        _serviceProvider = serviceProvider;
    }

    public MenuDbContext(
        DbContextOptions<MenuDbContext> options,
        ICurrentUser currentUser,
        IServiceProvider serviceProvider,
        ICompanyContext companyContext) : this(options, currentUser, serviceProvider)
    {
        _companyContext = companyContext;
    }

    public Guid CompanyId => _companyContext.CompanyId
        ?? throw new InvalidOperationException("A valid restaurant context is required for Menu data access.");

    public Guid RestaurantId => _companyContext.RestaurantId
        ?? throw new InvalidOperationException("A valid restaurant context is required for Menu data access.");

    public DbSet<Product> Products { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Ingredient> Ingredients { get; set; }

    public DbSet<RestaurantProductOffer> RestaurantProductOffers { get; set; }

    public DbSet<RestaurantIngredientPrice> RestaurantIngredientPrices { get; set; }

    public DbSet<MenuCard> MenuCards { get; set; }

    public DbSet<MenuSection> MenuSections { get; set; }

    public DbSet<MenuSectionItem> MenuSectionItems { get; set; }

    public IProductRepository Product => _serviceProvider.GetService<IProductRepository>() ?? throw new InvalidOperationException("ProductRepository is not registered in the service provider.");

    public ICategoryRepository Category => _serviceProvider.GetService<ICategoryRepository>() ?? throw new InvalidOperationException("CategoryRepository is not registered in the service provider.");

    public IIngredientRepository Ingredient => _serviceProvider.GetService<IIngredientRepository>() ?? throw new InvalidOperationException("IngredientRepository is not registered in the service provider.");

    public IRestaurantProductOfferRepository RestaurantProductOffer => _serviceProvider.GetService<IRestaurantProductOfferRepository>() ?? throw new InvalidOperationException("RestaurantProductOfferRepository is not registered in the service provider.");

    public IRestaurantIngredientPriceRepository RestaurantIngredientPrice => _serviceProvider.GetService<IRestaurantIngredientPriceRepository>() ?? throw new InvalidOperationException("RestaurantIngredientPriceRepository is not registered in the service provider.");

    public IMenuCardRepository MenuCard => _serviceProvider.GetService<IMenuCardRepository>() ?? throw new InvalidOperationException("MenuCardRepository is not registered in the service provider.");

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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ValidateCompanyOwnership();
        UpdateEntities();
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("menu");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MenuDbContext).Assembly, type => type.Namespace != null && type.Namespace.Contains("Menu.Configurations", StringComparison.Ordinal));
        modelBuilder.Entity<Product>().HasQueryFilter(entity => _companyContext.CompanyId.HasValue && entity.CompanyId == _companyContext.CompanyId.Value);
        modelBuilder.Entity<Category>().HasQueryFilter(entity => _companyContext.CompanyId.HasValue && entity.CompanyId == _companyContext.CompanyId.Value);
        modelBuilder.Entity<Ingredient>().HasQueryFilter(entity => _companyContext.CompanyId.HasValue && entity.CompanyId == _companyContext.CompanyId.Value);
        modelBuilder.Entity<RestaurantProductOffer>().HasQueryFilter(entity =>
            _companyContext.CompanyId.HasValue
            && entity.CompanyId == _companyContext.CompanyId.Value
            && _companyContext.RestaurantId.HasValue
            && entity.RestaurantId == _companyContext.RestaurantId.Value);
        modelBuilder.Entity<RestaurantIngredientPrice>().HasQueryFilter(entity =>
            _companyContext.RestaurantId.HasValue
            && entity.RestaurantId == _companyContext.RestaurantId.Value);
        modelBuilder.Entity<MenuCard>().HasQueryFilter(entity =>
            _companyContext.RestaurantId.HasValue
            && entity.RestaurantId == _companyContext.RestaurantId.Value);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<ProductId>().HaveConversion<ProductIdConverter>();
        configurationBuilder.Properties<CategoryId>().HaveConversion<CategoryIdConverter>();
        configurationBuilder.Properties<IngredientId>().HaveConversion<IngredientIdConverter>();
        configurationBuilder.Properties<UserId>().HaveConversion<UserIdConverter>();
        configurationBuilder.Properties<RestaurantProductOfferId>().HaveConversion<RestaurantProductOfferIdConverter>();
        configurationBuilder.Properties<RestaurantIngredientPriceId>().HaveConversion<RestaurantIngredientPriceIdConverter>();
        configurationBuilder.Properties<MenuCardId>().HaveConversion<MenuCardIdConverter>();
        configurationBuilder.Properties<MenuSectionId>().HaveConversion<MenuSectionIdConverter>();
        configurationBuilder.Properties<MenuSectionItemId>().HaveConversion<MenuSectionItemIdConverter>();
    }

    private void UpdateEntities()
    {
        DateTime currentDateTime = DateTime.UtcNow;

        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _currentUser.UserId.ToString();
                    entry.Entity.CreatedOn = currentDateTime;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedBy = _currentUser.UserId.ToString();
                    entry.Entity.ModifiedOn = currentDateTime;
                    break;
            }
        }
    }

    private void ValidateCompanyOwnership()
    {
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
        {
            Guid? entityCompanyId = entry.Entity switch
            {
                Product product => product.CompanyId,
                Category category => category.CompanyId,
                Ingredient ingredient => ingredient.CompanyId,
                RestaurantProductOffer offer => offer.CompanyId,
                _ => null
            };

            if (entityCompanyId.HasValue && entityCompanyId != _companyContext.CompanyId)
            {
                throw new InvalidOperationException("Menu data must be written within its owning company context.");
            }

            Guid? entityRestaurantId = entry.Entity switch
            {
                RestaurantProductOffer offer => offer.RestaurantId,
                RestaurantIngredientPrice price => price.RestaurantId,
                _ => null
            };

            if (entityRestaurantId.HasValue && entityRestaurantId != _companyContext.RestaurantId)
            {
                throw new InvalidOperationException("Restaurant pricing must be written within its owning restaurant context.");
            }
        }
    }
}
