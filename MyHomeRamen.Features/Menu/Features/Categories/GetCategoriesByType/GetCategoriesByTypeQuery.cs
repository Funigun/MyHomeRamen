using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;
using MyHomeRamen.Features.Common.Endpoints.Query;

namespace MyHomeRamen.Features.Menu.Features.Categories.GetCategoriesByType;

public sealed record GetCategoriesByTypeQuery(int CategoryType) : IQuery<IEnumerable<GetCategoriesByTypeResponse>>;
