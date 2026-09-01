using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Categories.GetMenuCategories;

public sealed record GetMenuCategoriesQuery : IQuery<GetMenuCategoriesResponse>;

public sealed record GetMenuCategoriesQueryOptions()
                   : DbQueryOptions<Category, CategoryForMenuDto>
                   (
                       new DbQueryOptions<Category, CategoryForMenuDto>
                       {
                           Selector = c => new(c.Id.Value, c.Name),
                           Filter = c => c.CategoryType == CategoryType.Product,
                       }
                   );

public sealed class GetMenuCategoriesHandler(IMenuDbContext dbContext)
                  : IRequestHandler<GetMenuCategoriesQuery, GetMenuCategoriesResponse>
{
    public async Task<GetMenuCategoriesResponse> Handle(GetMenuCategoriesQuery query, CancellationToken cancellationToken)
    {
        GetMenuCategoriesQueryOptions options = new();

        IEnumerable<CategoryForMenuDto> categories = await dbContext.Category.Query().GetMenuCategories(options, cancellationToken);

        return new GetMenuCategoriesResponse(categories);
    }
}
