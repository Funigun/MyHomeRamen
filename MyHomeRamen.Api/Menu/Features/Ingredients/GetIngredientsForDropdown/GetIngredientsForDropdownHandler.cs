using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForDropdown;

public sealed class GetIngredientsForDropdownHandler(IMenuDbContext dbContext)
    : IRequestHandler<GetIngredientsForDropdownQuery, IEnumerable<GetIngredientsForDropdownResponse>>
{
    public async Task<IEnumerable<GetIngredientsForDropdownResponse>> Handle(
        GetIngredientsForDropdownQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Ingredients
            .ForDropdown()
            .Select(i => i.ToResponse())
            .ToListAsync(cancellationToken);
    }
}
