using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Menu.Features.Categories.DeleteCategory;

public sealed class DeleteCategoryHandler(IMenuDbContext dbContext) : ICommandHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand id, CancellationToken cancellationToken)
    {
        Category category = await dbContext.Categories.GetById((CategoryId)id.Id, cancellationToken);

        dbContext.Categories.Remove(category);
        await ReorderCategories(category.Id, category.CategoryType, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
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
