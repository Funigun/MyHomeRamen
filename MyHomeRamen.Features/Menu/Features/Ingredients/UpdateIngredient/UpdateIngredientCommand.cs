using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.UpdateIngredient;

public sealed record UpdateIngredientCommand(IngredientId Id, UpdateIngredientRequest UpdateIngredientRequest)
                   : ICommand<UpdateIngredientResponse>;
