using MyHomeRamen.Api.Common.Endpoint.Pipeline;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.DeleteIngredient;

public record struct DeleteIngredientCommand : ICommand<IResult>
{
    public Guid Id { get; set; }
}
