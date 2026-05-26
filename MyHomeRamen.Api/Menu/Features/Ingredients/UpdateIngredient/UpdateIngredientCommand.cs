using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient;

public sealed record UpdateIngredientCommand(IngredientId Id, UpdateIngredientRequest UpdateIngredientRequest)
                   : ICommand<UpdateIngredientResponse>;
