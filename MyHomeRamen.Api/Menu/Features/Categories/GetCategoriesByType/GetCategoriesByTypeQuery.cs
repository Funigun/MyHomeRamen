using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType;

public sealed record GetCategoriesByTypeQuery(int CategoryType) : IQuery<IEnumerable<GetCategoriesByTypeResponse>>;
