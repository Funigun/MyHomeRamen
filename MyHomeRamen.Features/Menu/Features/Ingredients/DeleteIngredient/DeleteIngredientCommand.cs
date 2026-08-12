using FluentValidation;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.DeleteIngredient;

public record DeleteIngredientCommand(Guid Id) : ICommand;

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

public sealed class DeleteIngredientHandler(IMenuDbContext dbContext) : ICommandHandler<DeleteIngredientCommand>
{
    public async Task Handle(DeleteIngredientCommand id, CancellationToken cancellationToken)
    {
        Ingredient ingredient = await dbContext.Ingredient.Specification().ById((IngredientId)id.Id, cancellationToken);

        dbContext.Ingredient.Delete(ingredient);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

