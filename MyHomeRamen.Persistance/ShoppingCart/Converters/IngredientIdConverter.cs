using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;

namespace MyHomeRamen.Persistance.ShoppingCart.Converters;

public class IngredientIdConverter : ValueConverter<IngredientId, Guid>
{
    public IngredientIdConverter() : base(id => id.Value, value => new IngredientId(value)) { }
}
