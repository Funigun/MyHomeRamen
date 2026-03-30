using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForDropdown.Models;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForDropdown;

public sealed class GetIngredientsForDropdownHandler(IMenuDbContext dbContext)
    : IRequestHandler<GetIngredientsForDropdownRequest, IEnumerable<GetIngredientsForDropdownResponse>>
{
    public async Task<IEnumerable<GetIngredientsForDropdownResponse>> Handle(
        GetIngredientsForDropdownRequest request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Ingredients
            .ForDropdown()
            .Select(i => i.ToResponse())
            .ToListAsync(cancellationToken);
    }
}
