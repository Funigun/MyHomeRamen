using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Products.Requests;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory;

public sealed record GetProductsByCategoryQuery(GetProductsByCategoryRequest Request) : IQuery<IEnumerable<GetProductsByCategoryResponse>>;
