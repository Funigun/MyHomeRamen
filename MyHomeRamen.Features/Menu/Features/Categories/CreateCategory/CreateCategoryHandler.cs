using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Categories.CreateCategory;

public sealed class CreateCategoryHandler(IMenuDbContext dbContext) : ICommandHandler<CreateCategoryCommand, CreateCategoryResponse>
{
    public async Task<CreateCategoryResponse> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        int nextSortOrder = await dbContext.Category.Query().GetNextSortOrder((CategoryType)command.CreateCategoryRequest.CategoryType, cancellationToken);

        Category category = command.CreateCategoryRequest.ToDomain(nextSortOrder);

        dbContext.Category.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateCategoryResponse(category.Id.Value);
    }
}
