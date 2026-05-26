using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType;

public sealed record GetCategoriesByTypeQuery(int CategoryType) : IQuery<IEnumerable<GetCategoriesByTypeResponse>>;
