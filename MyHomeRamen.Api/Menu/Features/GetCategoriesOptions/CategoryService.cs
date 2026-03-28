namespace MyHomeRamen.Api.Menu.Features.GetCategoriesOptions;

public sealed class CategoryService : ICategoryService
{
    private static readonly IReadOnlyCollection<CategoryOption> Categories =
    [
        new(new Guid("11111111-1111-1111-1111-111111111111"), "Ramen"),
        new(new Guid("22222222-2222-2222-2222-222222222222"), "Side Dishes"),
        new(new Guid("33333333-3333-3333-3333-333333333333"), "Drinks"),
        new(new Guid("44444444-4444-4444-4444-444444444444"), "Desserts"),
        new(new Guid("55555555-5555-5555-5555-555555555555"), "Appetizers"),
    ];

    public Task<IReadOnlyCollection<CategoryOption>> GetCategoriesOptionsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Categories);
    }
}
