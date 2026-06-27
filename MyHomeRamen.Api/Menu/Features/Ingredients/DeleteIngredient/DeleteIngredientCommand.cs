using MyHomeRamen.Features.Common.Endpoints.Command;
namespace MyHomeRamen.Api.Menu.Features.Ingredients.DeleteIngredient;

public record DeleteIngredientCommand(Guid Id) : ICommand;
