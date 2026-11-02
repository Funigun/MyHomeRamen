using System.Collections.ObjectModel;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;
using MyHomeRamen.Persistance.Menu;

namespace MyHomeRamen.MenuApi.IntegrationTests.Companies;

public sealed class CompanyCatalogIsolationTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>
{
    [Fact]
    public async Task MenuCatalog_ShouldIsolateReadsAndWrites_ByCompany()
    {
        Guid otherCompanyId = Guid.CreateVersion7();
        Guid otherRestaurantId = Guid.CreateVersion7();
        Category otherCategory = Category.Create(
            new CategoryId(Guid.CreateVersion7()),
            otherCompanyId,
            "Foreign category",
            1,
            CategoryType.Product);
        Product otherProduct = Product.Create(
            new ProductId(Guid.CreateVersion7()),
            otherCompanyId,
            "Foreign ramen product",
            "Product from a different company.",
            string.Empty,
            new Collection<Ingredient>(),
            new Collection<Ingredient>(),
            [otherCategory]);

        await using (MenuDbContext otherCompanyContext = apiFactory.CreateMenuDbContextForTenant(otherRestaurantId, otherCompanyId))
        {
            otherCompanyContext.Categories.Add(otherCategory);
            otherCompanyContext.Products.Add(otherProduct);
            await otherCompanyContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        bool foreignProductVisible = await apiFactory.MenuDbContext.Product.Exists(
            product => product.Id == otherProduct.Id,
            TestContext.Current.CancellationToken);
        Assert.False(foreignProductVisible);

        await using MenuDbContext wrongCompanyWriteContext = apiFactory.CreateMenuDbContextForTenant(
            apiFactory.RestaurantId,
            TestTenant.CompanyId);
        Category foreignWrite = Category.Create(
            new CategoryId(Guid.CreateVersion7()),
            otherCompanyId,
            "Rejected foreign category",
            1,
            CategoryType.Product);
        wrongCompanyWriteContext.Categories.Add(foreignWrite);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            wrongCompanyWriteContext.SaveChangesAsync(TestContext.Current.CancellationToken));
    }
}
