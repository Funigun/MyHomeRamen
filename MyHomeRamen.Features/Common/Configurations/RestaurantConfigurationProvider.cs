using Microsoft.Extensions.Configuration;

namespace MyHomeRamen.Features.Common.Configurations;

public class RestaurantConfigurationProvider(IConfiguration configuration)
{
    private const string SectionKey = "RestaurantConfiguration:";

    public string InfrastructurePrefix => configuration.GetValue<string>($"{SectionKey}InfrastructurePrefix")!;

    public string RestaurantName => configuration[$"{SectionKey}Name"]!;

    public Guid RestaurantId => configuration.GetValue<Guid>($"{SectionKey}RestaurantId");
}
