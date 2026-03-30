using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForDropdown.Models;

public sealed record GetIngredientsForDropdownRequest : IRequest<IEnumerable<GetIngredientsForDropdownResponse>>;
