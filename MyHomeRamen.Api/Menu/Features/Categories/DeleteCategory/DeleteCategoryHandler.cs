using MyHomeRamen.Api.Common.Cache;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.Caching;
using MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory;

public sealed class DeleteCategoryHandler(IMenuDbContext dbContext, ICacheService cacheService) : IRequestHandler<DeleteCategoryRequest, IResult>
{
    public async Task<IResult> Handle(DeleteCategoryRequest id, CancellationToken cancellationToken)
    {
        Category category = await dbContext.Categories.GetById((CategoryId)id.Id, cancellationToken);

        dbContext.Categories.Remove(category);
        await ReorderCategories(category.Id, category.CategoryType, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        await ClearCache(category, cancellationToken);

        return Results.NoContent();
    }

    private async Task ReorderCategories(CategoryId idToSkip, CategoryType categoryType, CancellationToken cancellationToken)
    {
        List<Category> remaining = await dbContext.Categories.GetRemainingForResequencingAsync(categoryType, idToSkip, cancellationToken);

        for (int i = 0; i < remaining.Count; i++)
        {
            remaining[i].UpdateSortOrder(i + 1);
        }
    }

    private async Task ClearCache(Category category, CancellationToken cancellationToken)
    {
        IEnumerable<Task> cacheClearance = CategoryCacheInvalidation.GetAffectedKeys(category)
                                                                    .Select(key => cacheService.RemoveByKeyAsync(key, cancellationToken));

        await Task.WhenAll(cacheClearance);
    }
}
