namespace MyHomeRamen.Domain.Orders;

public sealed class Ingredient
{
    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    private Ingredient()
    {
    }

    public static Ingredient Create(string name, string description, decimal price)
    {
        return new Ingredient
        {
            Name = name,
            Description = description,
            Price = price
        };
    }
}
