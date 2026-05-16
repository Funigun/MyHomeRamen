using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory;

public sealed class DeleteCategoryHandler(IMenuDbContext dbContext) : ICommandHandler<DeleteCategoryCommand, IResult>
{
    public async Task<IResult> Handle(DeleteCategoryCommand id, CancellationToken cancellationToken)
    {
        Category category = await dbContext.Categories.GetById((CategoryId)id.Id, cancellationToken);

        dbContext.Categories.Remove(category);
        await ReorderCategories(category.Id, category.CategoryType, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

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
}
