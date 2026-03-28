using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForDropdown.Models;

public sealed record GetCategoriesForDropdownRequest(int CategoryType) : IRequest<IEnumerable<GetCategoriesForDropdownResponse>>;
