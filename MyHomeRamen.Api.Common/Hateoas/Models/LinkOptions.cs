namespace MyHomeRamen.Api.Common.Hateoas.Models;

public sealed class LinkOptions
{
    public required string Endpoint { get; set; }

    public required string Rel { get; set; }

    public required bool IsActionAllowed { get; set; }

    private LinkOptions()
    {
    }

    public static LinkOptions Create(string endpoint, string rel, bool isActionAllowed)
    {
        return new()
        {
            Endpoint = endpoint,
            Rel = rel,
            IsActionAllowed = isActionAllowed,
        };
    }
}
