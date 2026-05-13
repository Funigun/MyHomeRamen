using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient;

public sealed record CreateIngredientCommand(CreateIngredientRequest CreateIngredientRequest) : IRequest<CreateIngredientResponse>;
