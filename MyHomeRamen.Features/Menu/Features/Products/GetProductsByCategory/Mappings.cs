using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductsByCategory;

internal static class Mappings
{
    public static GetProductsByCategoryResponse ToResponse(this Product product)
    {
        return new GetProductsByCategoryResponse(
            product.Id.Value,
            product.Name,
            product.Description,
            product.Price,
            product.ImageUrl,
            product.BaseIngredients.Select(i => new ProductIngredientDto(i.Id.Value, i.Name)));
    }
}
