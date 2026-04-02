namespace MyHomeRamen.Blazor.Common.Configuration;

public class RestaurantConfiguration(IConfiguration configuration)
{
    private const string SectionKey = "RestaurantConfiguration:";

    public string InfrastructurePrefix => configuration.GetValue<string>($"{SectionKey}InfrastructurePrefix")!;

    public string RestaurantName => configuration[$"{SectionKey}Name"]!;

    public Guid RestaurantId => configuration.GetValue<Guid>($"{SectionKey}RestaurantId");

    public LayoutType LayoutType => configuration.GetValue<LayoutType>($"{SectionKey}LayoutType");

    public string Tagline => configuration[$"{SectionKey}Tagline"] ?? string.Empty;

    public string Description => configuration[$"{SectionKey}Description"] ?? string.Empty;

    public string EstablishedYear => configuration[$"{SectionKey}EstablishedYear"] ?? string.Empty;

    public string SeasonLabel => configuration[$"{SectionKey}SeasonLabel"] ?? string.Empty;

    public string Copyright => configuration[$"{SectionKey}Copyright"] ?? string.Empty;

    public string LocationAddress => configuration[$"{SectionKey}Location:Address"] ?? string.Empty;

    public string? LocationMapImageUrl => configuration[$"{SectionKey}Location:MapImageUrl"];
}
