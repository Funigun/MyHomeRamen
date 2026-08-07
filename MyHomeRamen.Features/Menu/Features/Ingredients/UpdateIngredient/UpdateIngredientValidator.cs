using FluentValidation;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.UpdateIngredient;

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
