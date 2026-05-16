using MyHomeRamen.Api.Common.Endpoint.Pipeline;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.DeleteIngredient;

public record DeleteIngredientCommand(Guid Id) : ICommand<IResult>;
