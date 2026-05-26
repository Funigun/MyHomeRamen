using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForDropdown;

public sealed record GetIngredientsForDropdownQuery : IQuery<IEnumerable<GetIngredientsForDropdownResponse>>;
