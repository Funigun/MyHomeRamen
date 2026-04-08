using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Api.Menu.Features.Products.UpdateProduct.Models;

internal static class Mappings
{
    internal static UpdateProductResponse ToResponse(this Product product)
        => new(product.Id.Value);
}
