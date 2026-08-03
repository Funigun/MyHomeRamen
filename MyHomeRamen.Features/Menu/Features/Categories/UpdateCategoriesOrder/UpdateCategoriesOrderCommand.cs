using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Categories.UpdateCategoriesOrder;

public sealed record UpdateCategoriesOrderCommand(UpdateCategoriesOrderRequest Request) : ICommand;

public sealed class UpdateCategoriesOrderHandler(IMenuDbContext dbContext) : ICommandHandler<UpdateCategoriesOrderCommand>
{
    public async Task Handle(UpdateCategoriesOrderCommand command, CancellationToken cancellationToken)
    {
        UpdateCategoriesOrderRequest request = command.Request;
        IEnumerable<CategoryId> ids = request.Items.Select(i => (CategoryId)i.Id);

        IEnumerable<Category> categories = await dbContext.Category.Specification().ByIds(ids, cancellationToken);

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
