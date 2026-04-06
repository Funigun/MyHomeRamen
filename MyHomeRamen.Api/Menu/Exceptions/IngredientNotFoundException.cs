using MyHomeRamen.Api.Common.Exceptions;

namespace MyHomeRamen.Api.Menu.Exceptions;

public sealed class IngredientNotFoundException(Guid id) : NotFoundException($"Ingredient with ID '{id}' was not found.");
