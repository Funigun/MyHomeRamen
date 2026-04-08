using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Products.UpdateProduct.Models;

public record struct UpdateProductRequestId : IRequestId<UpdateProductRequestId>
{
    public Guid Id { get; set; }
}
