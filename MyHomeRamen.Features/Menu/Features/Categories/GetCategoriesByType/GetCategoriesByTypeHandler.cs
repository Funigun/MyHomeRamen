using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;

namespace MyHomeRamen.Features.Menu.Features.Categories.GetCategoriesByType;

public sealed class GetCategoriesByTypeHandler(IMenuDbContext dbContext)
                  : IQueryHandler<GetCategoriesByTypeQuery, IEnumerable<GetCategoriesByTypeResponse>>
{
    public async Task<IEnumerable<GetCategoriesByTypeResponse>> Handle(GetCategoriesByTypeQuery request, CancellationToken cancellationToken)
    {
        GetCategoryByTypeQueryOptions queryOptions = new((CategoryType)request.CategoryType, c => new(c.Id, c.Name, c.SortOrder));

        IEnumerable<CategoryByTypeDto> categories = await dbContext.Category.Query().GetByTypeDto(queryOptions, cancellationToken);

        return categories.Select(c => new GetCategoriesByTypeResponse(c.Id.Value, c.Name, c.SortOrder));
    }
}

