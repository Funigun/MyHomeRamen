namespace MyHomeRamen.Blazor.Common.Configuration;

public sealed class ThemeConfiguration
{
    public PaletteConfiguration PaletteLight { get; set; } = new();

    public PaletteConfiguration PaletteDark { get; set; } = new();

    public TypographyConfiguration Typography { get; set; } = new();
}

public sealed class PaletteConfiguration
{
    public string? Primary { get; set; }

    public string? Secondary { get; set; }

    public string? AppbarBackground { get; set; }

    public string? Tertiary { get; set; }

    public string? Background { get; set; }

    public string? Surface { get; set; }

    public string? TextPrimary { get; set; }

    public string? TextSecondary { get; set; }

    public string? DrawerBackground { get; set; }

    public string? DrawerText { get; set; }

    public string? Error { get; set; }

    public string? AppbarText { get; set; }
}

public sealed class TypographyConfiguration
{
    public string HeadlineFont { get; set; } = "Newsreader, serif";

    public string BodyFont { get; set; } = "Be Vietnam Pro, sans-serif";

    public string LabelFont { get; set; } = "Be Vietnam Pro, sans-serif";
}
