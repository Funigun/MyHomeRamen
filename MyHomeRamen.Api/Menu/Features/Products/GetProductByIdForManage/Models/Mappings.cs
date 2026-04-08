using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductByIdForManage.Models;

internal static class Mappings
{
    public static GetProductByIdForManageResponse ToResponse(this Product product)
    {
        return new(
            product.Id.Value,
            product.Name,
            product.Description,
            product.Price,
            product.Categories.First().Id.Value,
            product.BaseIngredients.Select(i => i.Id.Value));
    }
}
