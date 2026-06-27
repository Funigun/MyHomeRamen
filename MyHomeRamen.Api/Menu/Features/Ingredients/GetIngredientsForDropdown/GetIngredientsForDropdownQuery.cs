using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForDropdown;

public sealed record GetIngredientsForDropdownQuery : IQuery<IEnumerable<GetIngredientsForDropdownResponse>>;
