using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType;

public sealed record GetCategoriesByTypeQuery(int CategoryType) : IRequest<IEnumerable<GetCategoriesByTypeResponse>>;
