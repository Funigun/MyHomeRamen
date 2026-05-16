using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductById;

public record struct GetProductByIdQuery : IQuery<GetProductByIdResponse>
{
    public Guid Id { get; set; }
}
