namespace MyHomeRamen.Api.Menu.Domain;

public sealed class Category
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    private Category()
    {
    }

    public static Category Create(Guid id, string name) =>
        new() { Id = id, Name = name };
}
