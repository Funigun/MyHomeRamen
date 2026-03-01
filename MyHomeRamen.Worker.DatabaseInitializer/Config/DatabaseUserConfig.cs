namespace MyHomeRamen.Worker.DatabaseInitializer.Config;

internal sealed record DatabaseUserConfig
{
    public string Schema { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;

    public string User { get; init; } = string.Empty;

    public string Password { get; init; }

    private DatabaseUserConfig(string schema, IConfiguration configuration)
    {
        Schema = schema.ToLower();
        Role = $"{schema}Role";
        User = configuration[$"DatabaseConfiguration:{schema}:User"]!;
        Password = configuration[$"DatabaseConfiguration:{schema}:Password"]!;
    }

    internal static DatabaseUserConfig Create(string schema, IConfiguration configuration)
    {
        return new DatabaseUserConfig(schema, configuration);
    }
}
