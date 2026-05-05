using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductById.Models;

internal static class Mappings
{
    public static GetProductByIdResponse ToResponse(this Product product) =>
        new(
            product.Id.Value,
            product.Name,
            product.Description,
            product.BaseIngredients.Select(i => new IngredientDto(i.Name, i.Description, i.Price)).ToList(),
            product.CustomIngredients.Select(i => new IngredientDto(i.Name, i.Description, i.Price)).ToList());
}
