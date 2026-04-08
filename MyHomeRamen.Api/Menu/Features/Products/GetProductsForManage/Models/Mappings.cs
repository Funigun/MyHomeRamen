using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsForManage.Models;

internal static class Mappings
{
    public static ProductDto ToResponse(this Product product)
    {
        return new(product.Id.Value, product.Name, product.Description, product.Price);
    }
}
