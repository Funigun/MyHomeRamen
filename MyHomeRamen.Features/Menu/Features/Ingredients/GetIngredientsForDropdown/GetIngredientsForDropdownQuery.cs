using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForDropdown;

public sealed record GetIngredientsForDropdownQuery : IQuery<IEnumerable<GetIngredientsForDropdownResponse>>;
