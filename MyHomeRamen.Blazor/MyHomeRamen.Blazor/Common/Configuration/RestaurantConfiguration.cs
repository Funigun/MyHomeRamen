namespace MyHomeRamen.Blazor.Common.Configuration;

public class RestaurantConfiguration(IConfiguration configuration)
{
    private const string SectionKey = "RestaurantConfiguration:";

    public string InfrastructurePrefix => configuration.GetValue<string>($"{SectionKey}InfrastructurePrefix")!;

    public string RestaurantName => configuration[$"{SectionKey}Name"]!;

    public Guid RestaurantId => configuration.GetValue<Guid>($"{SectionKey}RestaurantId");

    public LayoutType LayoutType => configuration.GetValue<LayoutType>($"{SectionKey}LayoutType");
}
