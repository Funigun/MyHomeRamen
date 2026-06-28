using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientById;

public record GetIngredientByIdQuery(Guid Id) : IQuery<GetIngredientByIdResponse>;
