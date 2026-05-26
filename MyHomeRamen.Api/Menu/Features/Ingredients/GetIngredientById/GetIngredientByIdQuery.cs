using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById;

public record GetIngredientByIdQuery(Guid Id) : IQuery<GetIngredientByIdResponse>;
