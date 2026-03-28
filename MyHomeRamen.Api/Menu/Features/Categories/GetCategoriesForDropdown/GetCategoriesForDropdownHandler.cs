using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForDropdown.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForDropdown;

public sealed class GetCategoriesForDropdownHandler(IMenuDbContext dbContext)
    : IRequestHandler<GetCategoriesForDropdownRequest, IEnumerable<GetCategoriesForDropdownResponse>>
{
    public async Task<IEnumerable<GetCategoriesForDropdownResponse>> Handle(
        GetCategoriesForDropdownRequest request,
        CancellationToken cancellationToken)
    {
        CategoryType categoryType = (CategoryType)request.CategoryType;

        return await dbContext.Categories
            .AsNoTracking()
            .Where(c => c.CategoryType == categoryType)
            .OrderBy(c => c.SortOrder)
            .Select(c => new GetCategoriesForDropdownResponse(c.Id.Value, c.Name))
            .ToListAsync(cancellationToken);
    }
}
