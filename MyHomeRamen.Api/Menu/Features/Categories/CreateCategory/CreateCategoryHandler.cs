using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Categories.CreateCategory;

public sealed class CreateCategoryHandler(IMenuDbContext dbContext) : ICommandHandler<CreateCategoryCommand, CreateCategoryResponse>
{
    public async Task<CreateCategoryResponse> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        int nextSortOrder = await dbContext.Categories.GetNextSortOrderAsync((CategoryType)command.CreateCategoryRequest.CategoryType, cancellationToken);

        Category category = command.CreateCategoryRequest.ToDomain(nextSortOrder);

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateCategoryResponse(category.Id.Value);
    }
}
