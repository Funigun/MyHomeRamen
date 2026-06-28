using MyHomeRamen.Features.Common.Endpoints.Command;
namespace MyHomeRamen.Features.Menu.Features.Ingredients.DeleteIngredient;

public record DeleteIngredientCommand(Guid Id) : ICommand;
