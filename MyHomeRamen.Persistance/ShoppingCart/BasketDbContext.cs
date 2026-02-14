using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyHomeRamen.Domain.ShoppingCart;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Persistance;
using MyHomeRamen.Persistance.ShoppingCart.Converters;

namespace MyHomeRamen.Persistance.ShoppingCart;

public class BasketDbContext : DbContext, IShoppingCartDbContext
{
    public BasketDbContext(DbContextOptions<BasketDbContext> options) : base(options) { }

    public DbSet<Basket> ShoppingCarts { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Ingredient> Ingredients { get; set; }

    public DbSet<User> Users { get; set; }

    public Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken)
    {
        return Database.BeginTransactionAsync(cancellationToken);
    }

    public Task CommitTransaction(CancellationToken cancellationToken)
    {
        return Database.CommitTransactionAsync(cancellationToken);
    }

    public Task RollbackTransaction(CancellationToken cancellationToken)
    {
        return Database.RollbackTransactionAsync(cancellationToken);
    }

    public async Task<bool> EnsureCreated(CancellationToken cancellationToken)
    {
        return await Database.EnsureCreatedAsync(cancellationToken);
    }

    public async Task Migrate(CancellationToken cancellationToken)
    {
        await Database.MigrateAsync(cancellationToken);
    }

    public async Task<int> ExecuteSql(FormattableString sql, CancellationToken cancellationToken)
    {
        return await Database.ExecuteSqlAsync(sql, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("basket");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BasketDbContext).Assembly, type => type.Namespace != null && type.Namespace.Contains("Basket.Configurations"));
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<BasketId>().HaveConversion<BasketIdConverter>();
        configurationBuilder.Properties<ProductId>().HaveConversion<ProductIdConverter>();
        configurationBuilder.Properties<IngredientId>().HaveConversion<IngredientIdConverter>();
        configurationBuilder.Properties<UserId>().HaveConversion<UserIdConverter>();
    }
}
