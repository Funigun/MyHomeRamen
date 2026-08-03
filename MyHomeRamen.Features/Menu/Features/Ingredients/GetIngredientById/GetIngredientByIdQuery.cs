using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientById;

public record GetIngredientByIdQuery(Guid Id) : IQuery<GetIngredientByIdResponse>;

public sealed class GetIngredientByIdHandler(IMenuDbContext dbContext)
    : IQueryHandler<GetIngredientByIdQuery, GetIngredientByIdResponse>
{
    public async Task<GetIngredientByIdResponse> Handle(GetIngredientByIdQuery request, CancellationToken cancellationToken)
    {
        IngredientId ingredientId = request.Id;

        Ingredient ingredient = await dbContext.Ingredient.Specification().ById(ingredientId, cancellationToken);

        return ingredient.ToResponse();
    }
}

