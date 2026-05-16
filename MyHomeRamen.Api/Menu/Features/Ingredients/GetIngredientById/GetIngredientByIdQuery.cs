using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById;

public record struct GetIngredientByIdQuery : IQuery<GetIngredientByIdResponse>
{
    public Guid Id { get; set; }
}
