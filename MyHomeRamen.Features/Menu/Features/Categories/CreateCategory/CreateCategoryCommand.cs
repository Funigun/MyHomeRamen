using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Categories.CreateCategory;

public sealed record CreateCategoryCommand(CreateCategoryRequest Request) : ICommand<CreateCategoryResponse>;

public sealed class CreateCategoryHandler(IMenuDbContext dbContext) : ICommandHandler<CreateCategoryCommand, CreateCategoryResponse>
{
    public async Task<CreateCategoryResponse> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        int nextSortOrder = await dbContext.Category.Query().GetNextSortOrder((CategoryType)command.Request.CategoryType, cancellationToken);

        Category category = command.Request.ToDomain(nextSortOrder);

        dbContext.Category.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateCategoryResponse(category.Id.Value);
    }
}

internal static class Mappings
{
    public static Category ToDomain(this CreateCategoryRequest request, int nextSortOrder)
        => Category.Create(Guid.NewGuid(), request.Name, nextSortOrder, (CategoryType)request.CategoryType);
}
