using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using FluentValidation;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.UpdateIngredient;

public sealed record UpdateIngredientCommand(IngredientId Id, UpdateIngredientRequest UpdateIngredientRequest)
                   : ICommand<UpdateIngredientResponse>;

public sealed class UpdateIngredientValidator : AbstractValidator<UpdateIngredientCommand>
{
    public UpdateIngredientValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Id.Value)
            .MustBeValidIngredientId(dbContext);

        RuleFor(x => x.UpdateIngredientRequest.Name)
            .MustMeetNameLengthRequirements();

        RuleFor(x => x.UpdateIngredientRequest.Description)
            .MustMeetDescriptionLengthRequirements();

        RuleFor(x => x.UpdateIngredientRequest.Price)
            .MustBeValidIngredientPrice();

        RuleFor(x => x)
            .MustHaveUniqueIngredientNameExcluding(dbContext, c => c.UpdateIngredientRequest.Name, c => c.Id)
            .OverridePropertyName(nameof(UpdateIngredientCommand.UpdateIngredientRequest) + "." + nameof(UpdateIngredientCommand.UpdateIngredientRequest.Name));

        RuleFor(x => x.UpdateIngredientRequest.CategoryIds)
            .NotEmpty()
            .WithMessage("At least one category must be selected.");
    }
}

public sealed class UpdateIngredientHandler(IMenuDbContext dbContext) : ICommandHandler<UpdateIngredientCommand, UpdateIngredientResponse>
{
    public async Task<UpdateIngredientResponse> Handle(UpdateIngredientCommand request, CancellationToken cancellationToken)
    {
        Ingredient ingredient = await dbContext.Ingredient.Specification().ById(request.Id, cancellationToken);

        IEnumerable<Category> categories = await dbContext.Category.Specification().ByIds(request.UpdateIngredientRequest.CategoryIds.Select(id => (CategoryId)id), cancellationToken);

        ingredient.Update(request.UpdateIngredientRequest.Name, request.UpdateIngredientRequest.Description, request.UpdateIngredientRequest.Price, categories);

        await dbContext.SaveChangesAsync(cancellationToken);

        return ingredient.ToResponse();
    }
}

internal static class Mappings
{
    internal static UpdateIngredientResponse ToResponse(this Ingredient ingredient)
        => new(ingredient.Id.Value);
}
