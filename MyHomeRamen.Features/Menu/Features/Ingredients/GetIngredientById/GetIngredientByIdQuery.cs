using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientById;

public record GetIngredientByIdQuery(Guid Id) : IQuery<GetIngredientByIdResponse>;

public sealed record GetIngredientByIdQueryOptions(IngredientId IngredientId)
                   : DbQueryOptions<Ingredient, IngredientByIdDto>
                   (
                       new()
                       {
                           Filter = ingredient => ingredient.Id == IngredientId,
                           Selector = ingredient => new IngredientByIdDto(
                               ingredient.Id.Value,
                               ingredient.Name,
                               ingredient.Description,
                               ingredient.Price,
                               ingredient.Categories.Select(category => category.Id.Value))
                       }
                   );

public sealed class GetIngredientByIdHandler(IMenuDbContext dbContext) : IQueryHandler<GetIngredientByIdQuery, GetIngredientByIdResponse>
{
    public async Task<GetIngredientByIdResponse> Handle(GetIngredientByIdQuery request, CancellationToken cancellationToken)
    {
        GetIngredientByIdQueryOptions options = new((IngredientId)request.Id);

        IngredientByIdDto? ingredient = await dbContext.Ingredient.Query().GetById(options, cancellationToken);

        return ingredient is null
            ? throw new InvalidOperationException("Ingredient was not found.")
            : ToResponse(ingredient);
    }

    private static GetIngredientByIdResponse ToResponse(IngredientByIdDto ingredient)
        => new(ingredient.Id, ingredient.Name, ingredient.Description, ingredient.Price, ingredient.CategoryIds);
}

