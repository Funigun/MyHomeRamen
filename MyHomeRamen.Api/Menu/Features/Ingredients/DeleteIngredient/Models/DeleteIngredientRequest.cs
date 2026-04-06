using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.DeleteIngredient.Models;

public record struct DeleteIngredientRequest : IRequestId<DeleteIngredientRequest>, IRequest<IResult>
{
    public Guid Id { get; set; }
}
