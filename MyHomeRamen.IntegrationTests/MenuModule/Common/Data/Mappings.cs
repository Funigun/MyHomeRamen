using MyHomeRamen.Api.Menu.Features.Products.CreateProduct.Models;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

internal static class Mappings
{
    internal static CreateProductRequest ToCreateProductRequest(this Product product) =>
        new(
            product.Name,
            product.Description,
            product.Price,
            product.Categories[0].Id,
            product.BaseIngredients.Select(i => (Guid)i.Id)
        );
}
