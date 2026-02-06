using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Persistance.Menu.Converters;

public class IngredientIdConverter : ValueConverter<IngredientId, Guid>
{
    public IngredientIdConverter() : base(id => id.Value, value => new IngredientId(value))
    {
    }
}
