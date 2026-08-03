using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductById;

internal static class Mappings
{
    public static GetProductByIdResponse ToResponse(this Product product) =>
        new(
            product.Id.Value,
            product.Name,
            product.Description,
            product.BaseIngredients.Select(i => new IngredientDto(i.Id.Value, i.Name, i.Description, i.Price)).ToList(),
            product.CustomIngredients.Select(i => new IngredientDto(i.Id.Value, i.Name, i.Description, i.Price)).ToList());
}
