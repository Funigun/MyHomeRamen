using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.CreateCategory.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Categories.CreateCategory;

public sealed class CreateCategoryHandler(IMenuDbContext dbContext) : IRequestHandler<CreateCategoryRequest, Guid>
{
    public async Task<Guid> Handle(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        int nextSortOrder = await dbContext.Categories.GetNextSortOrderAsync((CategoryType)request.CategoryType, cancellationToken);

        Category category = request.ToDomain(nextSortOrder);

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return category.Id.Value;
    }
}
