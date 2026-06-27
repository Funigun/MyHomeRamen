using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetMenuCategories;

public sealed record GetMenuCategoriesQuery : IQuery<IEnumerable<GetMenuCategoriesResponse>>;
