using System.Linq.Expressions;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Categories.GetCategoriesByType;

public sealed record GetCategoriesByTypeQuery(GetCategoriesByTypeRequest Request) : IQuery<GetCategoriesByTypeResponse>;

public sealed class GetCategoriesByTypeHandler(IMenuDbContext dbContext)
                  : IQueryHandler<GetCategoriesByTypeQuery, GetCategoriesByTypeResponse>
{
    public async Task<GetCategoriesByTypeResponse> Handle(GetCategoriesByTypeQuery query, CancellationToken cancellationToken)
    {
        GetCategoryByTypeQueryOptions options = new((CategoryType)query.Request.CategoryType);

        IEnumerable<CategoryByTypeDto> categories = await dbContext.Category.Query().GetByTypeDto(options, cancellationToken);

        return new GetCategoriesByTypeResponse(categories);
    }
}
