using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory;

public sealed class DeleteCategoryHandler(IMenuDbContext dbContext) : IRequestHandler<DeleteCategoryRequest, IResult>
{
    public async Task<IResult> Handle([FromRoute] DeleteCategoryRequest id, CancellationToken cancellationToken)
    {
        Category category = await dbContext.Categories.GetBySelectorAsync((CategoryId)id.Id, cancellationToken);

        dbContext.Categories.Remove(category);

        List<Category> remaining = await dbContext.Categories.GetRemainingForResequencingAsync(category.CategoryType, (CategoryId)id.Id, cancellationToken);
        for (int i = 0; i < remaining.Count; i++)
        {
            remaining[i].UpdateSortOrder(i + 1);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
