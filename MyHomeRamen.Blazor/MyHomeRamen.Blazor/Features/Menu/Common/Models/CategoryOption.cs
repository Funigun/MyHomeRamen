namespace MyHomeRamen.Blazor.Features.Menu.Common.Models;

public sealed record CategoryOption(Guid Id, string Name)
{
    public override string ToString() => Name;
}
