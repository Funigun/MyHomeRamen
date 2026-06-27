using MyHomeRamen.Common.Contracts.Menu.Products.DTOs;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsForManage;

internal static class Mappings
{
    public static ProductForManageDto ToResponse(this Product product)
    {
        return new(product.Id.Value, product.Name, product.Description, product.Price);
    }
}
