namespace MyHomeRamen.Identity.Api.Application.Services;

public class RestaurantConfigurationFactory(IConfiguration configuration)
{
    private const string SectionKey = "RestaurantConfiguration";

    public RestaurantConfiguration Create()
    {
        IConfigurationSection section = configuration.GetSection(SectionKey);

        return RestaurantConfiguration.Create
        (
            configuration.GetConnectionString(section.GetValue<string>("ConnectionStringResourceName")!) ?? string.Empty,
            section.GetValue<Guid>("RestaurantId")
        );
    }
}
