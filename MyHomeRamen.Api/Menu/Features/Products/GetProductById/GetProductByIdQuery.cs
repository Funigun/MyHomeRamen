using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductById;

public record struct GetProductByIdQuery : IRequestId<GetProductByIdQuery>, IRequest<GetProductByIdResponse>
{
    public Guid Id { get; set; }
}
