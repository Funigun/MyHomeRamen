using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Domain.Restaurants.Restaurants;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;
using MyHomeRamen.Features.Restaurants.Features.Companies.Common;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;
using MyHomeRamen.Persistance.Restaurants.Converters;

namespace MyHomeRamen.Persistance.Restaurants;

public sealed class RestaurantsDbContext(DbContextOptions<RestaurantsDbContext> options) : DbContext(options), IRestaurantDbContext
{
    private readonly ICurrentUser _currentUser = default!;
    private readonly IServiceProvider _serviceProvider = default!;

    public RestaurantsDbContext(DbContextOptions<RestaurantsDbContext> options, ICurrentUser currentUser) : this(options)
    {
        _currentUser = currentUser;
    }

    public RestaurantsDbContext(DbContextOptions<RestaurantsDbContext> options, ICurrentUser currentUser, IServiceProvider serviceProvider) : this(options)
    {
        _currentUser = currentUser;
        _serviceProvider = serviceProvider;
    }

    public DbSet<CompanyDetails> Companies { get; set; }

    public DbSet<Restaurant> Restaurants { get; set; }

    public ICompanyRepository Company => _serviceProvider.GetService<ICompanyRepository>() ?? throw new InvalidOperationException("CompanyRepository is not registered in the service provider.");

    public IRestaurantRepository Restaurant => _serviceProvider.GetService<IRestaurantRepository>() ?? throw new InvalidOperationException("RestaurantRepository is not registered in the service provider.");

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("restaurants");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RestaurantsDbContext).Assembly, type => type.Namespace != null && type.Namespace.Contains("Restaurants.Configurations", StringComparison.Ordinal));
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<CompanyDetailsId>().HaveConversion<CompanyDetailsIdConverter>();
        configurationBuilder.Properties<SocialMediaId>().HaveConversion<SocialMediaIdConverter>();
        configurationBuilder.Properties<RestaurantId>().HaveConversion<RestaurantIdConverter>();
        configurationBuilder.Properties<ClosingPeriodId>().HaveConversion<ClosingPeriodIdConverter>();
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
