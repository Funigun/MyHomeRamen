using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientById;
using MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForDropdown;
using MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForManage;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.Common;

public interface IIngredientQuery
{
    Task<IngredientByIdDto?> GetById(GetIngredientByIdQueryOptions options, CancellationToken cancellationToken);

    Task<IEnumerable<IngredientForDropdownDto>> ForDropdown(GetIngredientsForDropdownQueryOptions options, CancellationToken cancellationToken);

    Task<PagedResult<IngredientForManageDto>> ForManage(GetIngredientsForManageQueryOptions options, CancellationToken cancellationToken);

    Task<bool> IsIngredientNameUnique(string name, CancellationToken cancellationToken);

    Task<bool> IsIngredientNameUniqueExcluding(string name, IngredientId excludeId, CancellationToken cancellationToken);
}
