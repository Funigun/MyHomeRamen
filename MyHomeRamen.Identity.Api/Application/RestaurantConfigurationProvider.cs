namespace MyHomeRamen.Identity.Api.Application;

public class RestaurantConfigurationProvider(IConfiguration configuration)
{
    private const string SectionKey = "RestaurantConfiguration:";

    public string RestaurantName => configuration[$"{SectionKey}Name"]!;

    public string ConnectionString => configuration.GetConnectionString(configuration[$"{SectionKey}ConnectionStringResourceName"]!)!;

    public Guid RestaurantId => configuration.GetValue<Guid>($"{SectionKey}RestaurantId");

    public string InfrastructurePrefix => configuration.GetValue<string>($"{SectionKey}InfrastructurePrefix")!;
}
