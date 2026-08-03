using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductsForManage;

internal static class Mappings
{
    public static ProductForManageDto ToResponse(this Product product)
    {
        return new(product.Id.Value, product.Name, product.Description, product.Price);
    }
}
