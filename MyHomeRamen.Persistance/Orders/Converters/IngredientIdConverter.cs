using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Orders;

namespace MyHomeRamen.Persistance.Orders.Converters;

public class IngredientIdConverter : ValueConverter<IngredientId, Guid>
{
    public IngredientIdConverter() : base(id => id.Value, value => new IngredientId(value)) { }
}
