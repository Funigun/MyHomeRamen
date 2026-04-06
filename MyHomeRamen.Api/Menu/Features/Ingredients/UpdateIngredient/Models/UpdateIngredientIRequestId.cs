using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient.Models;

public record struct UpdateIngredientIRequestId : IRequestId<UpdateIngredientIRequestId>
{
    public Guid Id { get; set; }
}
