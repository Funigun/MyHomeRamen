using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.CreateIngredient;

public sealed record CreateIngredientCommand(CreateIngredientRequest CreateIngredientRequest) : ICommand<CreateIngredientResponse>;
