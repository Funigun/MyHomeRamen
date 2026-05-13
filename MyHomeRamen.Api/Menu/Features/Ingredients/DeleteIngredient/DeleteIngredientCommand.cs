using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.DeleteIngredient;

public record struct DeleteIngredientCommand : IRequestId<DeleteIngredientCommand>, IRequest<IResult>
{
    public Guid Id { get; set; }
}
