using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory;

public sealed class DeleteCategoryHandler(IMenuDbContext dbContext) : IRequestHandler<DeleteCategoryRequest, IResult>
{
    public async Task<IResult> Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
    {
        Category? category = await dbContext.Categories
            .FindByIdAsync<Category, CategoryId>((CategoryId)request.Id, cancellationToken);

        if (category is null)
        {
            return Results.NotFound();
        }

        bool isInUse = category.CategoryType == CategoryType.Product
            ? await dbContext.Products.IsCategoryUsedByProductAsync((CategoryId)request.Id, cancellationToken)
            : await dbContext.Ingredients.IsCategoryUsedByIngredientAsync((CategoryId)request.Id, cancellationToken);

        if (isInUse)
        {
            return Results.Conflict("Category is still in use and cannot be deleted.");
        }

        dbContext.Categories.Remove(category);

        List<Category> remaining = await dbContext.Categories
            .GetRemainingForResequencingAsync(category.CategoryType, (CategoryId)request.Id, cancellationToken);

        for (int i = 0; i < remaining.Count; i++)
        {
            remaining[i].UpdateSortOrder(i + 1);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
