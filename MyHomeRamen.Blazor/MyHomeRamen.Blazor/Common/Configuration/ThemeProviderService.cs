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
            Typography = MapTypography(config.Typography),
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

        if (config.Tertiary is not null)
        {
            palette.Tertiary = config.Tertiary;
        }

        if (config.Background is not null)
        {
            palette.Background = config.Background;
        }

        if (config.Surface is not null)
        {
            palette.Surface = config.Surface;
        }

        if (config.TextPrimary is not null)
        {
            palette.TextPrimary = config.TextPrimary;
        }

        if (config.TextSecondary is not null)
        {
            palette.TextSecondary = config.TextSecondary;
        }

        if (config.DrawerBackground is not null)
        {
            palette.DrawerBackground = config.DrawerBackground;
        }

        if (config.DrawerText is not null)
        {
            palette.DrawerText = config.DrawerText;
        }

        if (config.Error is not null)
        {
            palette.Error = config.Error;
        }

        if (config.AppbarText is not null)
        {
            palette.AppbarText = config.AppbarText;
        }

        return palette;
    }

    private static Typography MapTypography(TypographyConfiguration config)
    {
        string[] headlineFonts = config.HeadlineFont.Split(',', StringSplitOptions.TrimEntries);
        string[] bodyFonts = config.BodyFont.Split(',', StringSplitOptions.TrimEntries);

        Typography typography = new();
        typography.Default.FontFamily = bodyFonts;
        typography.H1.FontFamily = headlineFonts;
        typography.H2.FontFamily = headlineFonts;
        typography.H3.FontFamily = headlineFonts;
        typography.H4.FontFamily = headlineFonts;
        typography.H5.FontFamily = headlineFonts;
        typography.H6.FontFamily = headlineFonts;

        return typography;
    }
}
