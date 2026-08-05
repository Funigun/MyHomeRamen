using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductById;

public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResponse>;

public sealed record GetProductByIdQueryOptions(ProductId ProductId)
                   : DbQueryOptions<Product, ProductByIdDto>
                   (
                       new()
                       {
                           Filter = product => product.Id == ProductId,
                           Selector = product => new ProductByIdDto(
                               product.Id.Value,
                               product.Name,
                               product.Description,
                               product.BaseIngredients.Select(ingredient => new IngredientDto(ingredient.Id.Value, ingredient.Name, ingredient.Description, ingredient.Price)).ToList(),
                               product.CustomIngredients.Select(ingredient => new IngredientDto(ingredient.Id.Value, ingredient.Name, ingredient.Description, ingredient.Price)).ToList())
                       }
                   );

public sealed class GetProductByIdHandler(IMenuDbContext dbContext)
    : IQueryHandler<GetProductByIdQuery, GetProductByIdResponse>
{
    public async Task<GetProductByIdResponse> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        ProductByIdDto? product = await dbContext.Product.Query().GetById(new GetProductByIdQueryOptions((ProductId)query.Id), cancellationToken);

        return product is null
            ? throw new InvalidOperationException("Product was not found.")
            : ToResponse(product);
    }

    private static GetProductByIdResponse ToResponse(ProductByIdDto product)
         => new(product.Id, product.Name, product.Description, product.BaseIngredients, product.CustomIngredients);
}

