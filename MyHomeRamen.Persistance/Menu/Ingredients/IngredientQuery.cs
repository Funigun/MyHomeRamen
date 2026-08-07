using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;
using MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientById;
using MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForDropdown;
using MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForManage;

namespace MyHomeRamen.Persistance.Menu;

public partial class IngredientRepository : IIngredientQuery
{
    public async Task<IngredientByIdDto?> GetById(GetIngredientByIdQueryOptions options, CancellationToken cancellationToken)
        => await QueryFirstOrDefault(
            menuDbContext.Ingredients
                .Include(i => i.Categories)
                .AsSplitQuery(),
            options,
            cancellationToken);

    public async Task<IEnumerable<IngredientForDropdownDto>> ForDropdown(GetIngredientsForDropdownQueryOptions options, CancellationToken cancellationToken)
        => await QueryList(menuDbContext.Ingredients, options, cancellationToken);

    public async Task<PagedResult<IngredientForManageDto>> ForManage(GetIngredientsForManageQueryOptions options, CancellationToken cancellationToken)
        => await QueryPaged(menuDbContext.Ingredients, options, cancellationToken);

    public async Task<bool> IsIngredientNameUnique(string name, CancellationToken cancellationToken)
        => !await Exists(i => i.Name.ToLower() == name.ToLower(), cancellationToken);

    public async Task<bool> IsIngredientNameUniqueExcluding(string name, IngredientId excludeId, CancellationToken cancellationToken)
        => !await Exists(i => i.Id != excludeId && i.Name.ToLower() == name.ToLower(), cancellationToken);
}
