using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForManage.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForManage;

public sealed class GetCategoriesForManageHandler(IMenuDbContext dbContext)
    : IRequestHandler<GetCategoriesForManageRequest, GetCategoriesForManageResponse>
{
    public async Task<GetCategoriesForManageResponse> Handle(
        GetCategoriesForManageRequest request,
        CancellationToken cancellationToken)
    {
        IEnumerable<CategoryForManageDto> productCategories = await dbContext.Categories
            .ForManage(CategoryType.Product)
            .Select(c => c.ToManageDto())
            .ToListAsync(cancellationToken);

        IEnumerable<CategoryForManageDto> ingredientCategories = await dbContext.Categories
            .ForManage(CategoryType.Ingredient)
            .Select(c => c.ToManageDto())
            .ToListAsync(cancellationToken);

        return new GetCategoriesForManageResponse(productCategories, ingredientCategories);
    }
}
