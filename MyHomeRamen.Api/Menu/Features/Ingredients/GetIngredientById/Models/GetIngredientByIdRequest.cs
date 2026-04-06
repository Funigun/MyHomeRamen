using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById.Models;

public record struct GetIngredientByIdRequest : IRequestId<GetIngredientByIdRequest>, IRequest<GetIngredientByIdResponse>
{
    public Guid Id { get; set; }
}
