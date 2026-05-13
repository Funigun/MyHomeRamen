using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Menu.Products.Requests;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory;

public sealed record GetProductsByCategoryQuery(GetProductsByCategoryRequest Request) : IRequest<IEnumerable<GetProductsByCategoryResponse>>;
