using MudBlazor;

namespace MyHomeRamen.Blazor.Common.Configuration;

public sealed class ThemeProviderService(IConfiguration configuration)
{
    private const string SectionKey = "ThemeConfiguration";

    public MudTheme GetTheme()
    {
        ThemeConfiguration config = new();
        configuration.GetSection(SectionKey).Bind(config);

        return new MudTheme
        {
            PaletteLight = MapPalette<PaletteLight>(config.PaletteLight),
            PaletteDark = MapPalette<PaletteDark>(config.PaletteDark),
        };
    }

    private static T MapPalette<T>(PaletteConfiguration config)
             where T : Palette, new()
    {
        T palette = new();

        if (config.Primary is not null)
        {
            palette.Primary = config.Primary;
        }

        if (config.Secondary is not null)
        {
            palette.Secondary = config.Secondary;
        }

        if (config.AppbarBackground is not null)
        {
            palette.AppbarBackground = config.AppbarBackground;
        }

        return palette;
    }
}
