namespace MyHomeRamen.Blazor.Common.Configuration;

public sealed class ThemeConfiguration
{
    public PaletteConfiguration PaletteLight { get; set; } = new();

    public PaletteConfiguration PaletteDark { get; set; } = new();
}

public sealed class PaletteConfiguration
{
    public string? Primary { get; set; }

    public string? Secondary { get; set; }

    public string? AppbarBackground { get; set; }
}
