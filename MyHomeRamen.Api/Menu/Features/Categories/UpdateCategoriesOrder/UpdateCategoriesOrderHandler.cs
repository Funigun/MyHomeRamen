using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.UpdateCategoriesOrder.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Categories.UpdateCategoriesOrder;

public sealed class UpdateCategoriesOrderHandler(IMenuDbContext dbContext) : IRequestHandler<UpdateCategoriesOrderRequest>
{
    public async Task Handle(UpdateCategoriesOrderRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<CategoryId> ids = request.Items.Select(i => (CategoryId)i.Id);

        IEnumerable<Category> categories = await dbContext.Categories
            .GetByIds<Category, CategoryId>(ids, cancellationToken);

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
}
