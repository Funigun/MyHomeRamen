using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Categories.GetMenuCategories;

public sealed record GetMenuCategoriesQuery : IQuery<GetMenuCategoriesResponse>;

public sealed class GetMenuCategoriesHandler(IMenuDbContext dbContext)
                  : IQueryHandler<GetMenuCategoriesQuery, GetMenuCategoriesResponse>
{
    public async Task<GetMenuCategoriesResponse> Handle(GetMenuCategoriesQuery query, CancellationToken cancellationToken)
    {
        GetMenuCategoriesQueryOptions options = new();

        IEnumerable<CategoryForMenuDto> categories = await dbContext.Category.Query().GetMenuCategories(options, cancellationToken);

        return new GetMenuCategoriesResponse(categories);
    }
}
