using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Features.Menu.Features.Products.UpdateProduct;

internal static class Mappings
{
    internal static UpdateProductResponse ToResponse(this Product product)
        => new(product.Id.Value);
}
