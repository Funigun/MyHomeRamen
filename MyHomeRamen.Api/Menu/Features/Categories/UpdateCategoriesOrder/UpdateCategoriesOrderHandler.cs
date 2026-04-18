using MyHomeRamen.Api.Common.Cache;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.Caching;
using MyHomeRamen.Api.Menu.Features.Categories.UpdateCategoriesOrder.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Categories.UpdateCategoriesOrder;

public sealed class UpdateCategoriesOrderHandler(IMenuDbContext dbContext, ICacheService cacheService) : IRequestHandler<UpdateCategoriesOrderRequest>
{
    public async Task Handle(UpdateCategoriesOrderRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<CategoryId> ids = request.Items.Select(i => (CategoryId)i.Id);

        IEnumerable<Category> categories = await dbContext.Categories.GetByIds(ids, cancellationToken);

        await ReorderCategories(categories, request, cancellationToken);

        await ClearCache(categories, cancellationToken);
    }

    private async Task ReorderCategories(IEnumerable<Category> categories, UpdateCategoriesOrderRequest request, CancellationToken cancellationToken)
    {
        Dictionary<CategoryId, Category> categoryMap = categories.ToDictionary(c => c.Id);

        foreach (CategoryOrderItemDto item in request.Items)
        {
            if (categoryMap.TryGetValue(item.Id, out Category? category))
            {
                category.UpdateSortOrder(item.SortOrder);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ClearCache(IEnumerable<Category> categoryTypes, CancellationToken cancellationToken)
    {
        IEnumerable<Task> cacheClearance = CategoryCacheInvalidation.GetAffectedKeys(categoryTypes)
                                                                    .Select(key => cacheService.RemoveByKeyAsync(key, cancellationToken));

        await Task.WhenAll(cacheClearance);
    }
}
