using FluentValidation;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;
using MyHomeRamen.Features.Common.Mediator;

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

public sealed class GetIngredientByIdAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetIngredientByIdQuery>
{
    public async Task<bool> Authorize(GetIngredientByIdQuery request, CancellationToken cancellationToken)
    {
        return currentUser.CanManageIngredients() && currentUser.CanEditIngredient();
    }
}

public sealed class GetIngredientByIdValidator : AbstractValidator<GetIngredientByIdQuery>
{
    public GetIngredientByIdValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Id)
            .MustBeValidIngredientId(dbContext);
    }
}

public sealed class GetIngredientByIdHandler(IMenuDbContext dbContext) : IRequestHandler<GetIngredientByIdQuery, GetIngredientByIdResponse>
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
