using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory.Models;

public sealed record GetProductsByCategoryRequest(Guid CategoryId) : IRequest<IEnumerable<GetProductsByCategoryResponse>>;
