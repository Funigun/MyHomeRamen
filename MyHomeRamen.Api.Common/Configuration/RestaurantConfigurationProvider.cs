using Microsoft.Extensions.Configuration;

namespace MyHomeRamen.Api.Common.Configuration;

public class RestaurantConfigurationProvider(IConfiguration configuration)
{
    private const string SectionKey = "RestaurantConfiguration:";

    public string InfrastructurePrefix => configuration.GetValue<string>($"{SectionKey}InfrastructurePrefix")!;

    public string RestaurantName => configuration[$"{SectionKey}Name"]!;

    public Guid RestaurantId => configuration.GetValue<Guid>($"{SectionKey}RestaurantId");

    public string? IdentityConnectionString => GetConnectionString("IdentityConnectionString");

    public string? MenuConnectionString => GetConnectionString("MenuConnectionString");

    public string? ReservationsConnectionString => GetConnectionString("ReservationConnectionString");

    public string? OrdersConnectionString => GetConnectionString("OrderConnectionString");

    public string? ShoppingCartConnectionString => GetConnectionString("ShoppingCartConnectionString");

    public string? PaymentsConnectionString => GetConnectionString("PaymentConnectionString");

    public string? WorkerConnectionString => GetConnectionString("WorkerConnectionString");

    private string? GetConnectionString(string key)
    {
        string? value = configuration[$"{SectionKey}{key}"];
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        if (configuration.GetSection(value).Exists())
        {
            return configuration[value];
        }

        return value;
    }
}
