using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductById.Models;

public record struct GetProductByIdRequest : IRequestId<GetProductByIdRequest>, IRequest<GetProductByIdResponse>
{
    public Guid Id { get; set; }
}
