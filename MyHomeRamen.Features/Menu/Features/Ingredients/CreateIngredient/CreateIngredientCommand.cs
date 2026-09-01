using System.Collections.ObjectModel;
using FluentValidation;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.CreateIngredient;

public sealed record CreateIngredientCommand(CreateIngredientRequest CreateIngredientRequest) : ICommand<CreateIngredientResponse>;

public sealed class CreateIngredientAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<CreateIngredientCommand>
{
    public async Task<bool> Authorize(CreateIngredientCommand request, CancellationToken cancellationToken)
    {
        return currentUser.CanManageIngredients() && currentUser.CanAddIngredient();
    }
}

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

public sealed class CreateIngredientHandler(IMenuDbContext dbContext) : ICommandHandler<CreateIngredientCommand, CreateIngredientResponse>
{
    public async Task<CreateIngredientResponse> Handle(CreateIngredientCommand command, CancellationToken cancellationToken)
    {
        IEnumerable<Category> categories = await dbContext.Category.Query()
                                                                   .GetByIds(command.CreateIngredientRequest.CategoryIds.Select(id => (CategoryId)id), cancellationToken);

        Ingredient ingredient = command.CreateIngredientRequest.ToDomain(categories);

        dbContext.Ingredient.Add(ingredient);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateIngredientResponse(ingredient.Id.Value);
    }
}

internal static class Mappings
{
    public static Ingredient ToDomain(this CreateIngredientRequest request, IEnumerable<Category> categories)
    {
        return Ingredient.Create(
            Guid.NewGuid(),
            request.Name,
            request.Description,
            request.Price,
            new Collection<Category>(categories.ToList()));
    }
}
