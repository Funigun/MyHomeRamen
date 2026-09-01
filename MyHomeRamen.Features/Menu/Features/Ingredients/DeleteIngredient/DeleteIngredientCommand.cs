using FluentValidation;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.DeleteIngredient;

public record DeleteIngredientCommand(Guid Id) : ICommand;

public sealed class DeleteIngredientAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<DeleteIngredientCommand>
{
    public async Task<bool> Authorize(DeleteIngredientCommand request, CancellationToken cancellationToken)
    {
        return currentUser.CanManageIngredients() && currentUser.CanDeleteIngredient();
    }
}

public sealed class DeleteIngredientValidator : AbstractValidator<DeleteIngredientCommand>
{
    public DeleteIngredientValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .MustBeValidIngredientId(dbContext)
            .MustNotBeUsedAsBaseIngredient(dbContext)
            .MustNotBeUsedAsCustomIngredient(dbContext);
    }
}

public sealed class DeleteIngredientHandler(IMenuDbContext dbContext) : IRequestHandler<DeleteIngredientCommand, Unit>
{
    public async Task<Unit> Handle(DeleteIngredientCommand id, CancellationToken cancellationToken)
    {
        Ingredient ingredient = await dbContext.Ingredient.Load().ById((IngredientId)id.Id, cancellationToken);

        dbContext.Ingredient.Delete(ingredient);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
