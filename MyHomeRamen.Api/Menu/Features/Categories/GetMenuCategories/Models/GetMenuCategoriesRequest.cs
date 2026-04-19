using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetMenuCategories.Models;

public sealed record GetMenuCategoriesRequest : IRequest<IEnumerable<GetMenuCategoriesResponse>>;
