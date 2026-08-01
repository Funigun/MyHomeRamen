using FluentValidation;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.CreateIngredient;

public sealed class CreateIngredientValidator : AbstractValidator<CreateIngredientCommand>
{
    public CreateIngredientValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.CreateIngredientRequest.Name)
            .MustMeetNameLengthRequirements()
            .MustHaveUniqueIngredientName(dbContext);

        RuleFor(x => x.CreateIngredientRequest.Description)
            .MustMeetDescriptionLengthRequirements();

        RuleFor(x => x.CreateIngredientRequest.Price)
            .MustBeValidIngredientPrice();

        RuleFor(x => x.CreateIngredientRequest.CategoryIds)
            .NotEmpty()
            .WithMessage("At least one category must be selected.");
    }
}
