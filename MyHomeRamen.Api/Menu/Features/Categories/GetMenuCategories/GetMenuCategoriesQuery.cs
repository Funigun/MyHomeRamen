using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetMenuCategories;

public sealed record GetMenuCategoriesQuery : IRequest<IEnumerable<GetMenuCategoriesResponse>>;
