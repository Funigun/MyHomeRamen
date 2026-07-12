using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Categories.GetCategoriesByType;

public sealed class GetCategoriesByTypeHandler(IMenuDbContext dbContext)
                  : IQueryHandler<GetCategoriesByTypeQuery, IEnumerable<GetCategoriesByTypeResponse>>
{
    public async Task<IEnumerable<GetCategoriesByTypeResponse>> Handle(GetCategoriesByTypeQuery request, CancellationToken cancellationToken)
    {
        CategoryType categoryType = (CategoryType)request.CategoryType;

        List<Category> categories = await dbContext.Category.Query()
                                                            .GetByType(categoryType, cancellationToken);

        return categories.Select(c => c.ToResponse());
    }
}
