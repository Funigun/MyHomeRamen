using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Menu.Categories.DTOs;
using MyHomeRamen.Common.Contracts.Menu.Categories.Requests;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Categories.UpdateCategoriesOrder;

public sealed class UpdateCategoriesOrderHandler(IMenuDbContext dbContext) : ICommandHandler<UpdateCategoriesOrderCommand>
{
    public async Task Handle(UpdateCategoriesOrderCommand command, CancellationToken cancellationToken)
    {
        UpdateCategoriesOrderRequest request = command.UpdateCategoriesOrderRequest;
        IEnumerable<CategoryId> ids = request.Items.Select(i => (CategoryId)i.Id);

        IEnumerable<Category> categories = await dbContext.Categories.GetByIds(ids, cancellationToken);

        await ReorderCategories(categories, request, cancellationToken);
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
}
