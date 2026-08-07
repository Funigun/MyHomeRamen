using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Categories.DeleteCategory;

public record DeleteCategoryCommand(DeleteCategoryRequest Request) : ICommand;

public sealed class DeleteCategoryHandler(IMenuDbContext dbContext) : ICommandHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        Category category = await dbContext.Category.Specification().ById((CategoryId)command.Request.Id, cancellationToken);

        dbContext.Category.Delete(category);

        List<Category> remaining = (await dbContext.Category.Specification().GetRemainingForResequencing(category.CategoryType, category.Id, cancellationToken)).ToList();

        for (int i = 0; i < remaining.Count; i++)
        {
            remaining[i].UpdateSortOrder(i + 1);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
