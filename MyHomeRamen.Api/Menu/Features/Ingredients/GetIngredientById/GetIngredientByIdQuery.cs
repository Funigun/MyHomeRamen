using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById;

public record struct GetIngredientByIdQuery : IRequestId<GetIngredientByIdQuery>, IRequest<GetIngredientByIdResponse>
{
    public Guid Id { get; set; }
}
