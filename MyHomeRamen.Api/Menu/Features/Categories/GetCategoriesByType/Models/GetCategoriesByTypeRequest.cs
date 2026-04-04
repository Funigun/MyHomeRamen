using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType.Models;

public sealed record GetCategoriesByTypeRequest(int CategoryType) : IRequest<IEnumerable<GetCategoriesByTypeResponse>>;
