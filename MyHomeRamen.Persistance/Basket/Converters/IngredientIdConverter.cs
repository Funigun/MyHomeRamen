using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Basket;

namespace MyHomeRamen.Persistance.Basket.Converters;

public class IngredientIdConverter : ValueConverter<IngredientId, Guid>
{
    public IngredientIdConverter() : base(id => id.Value, value => new IngredientId(value)) { }
}
