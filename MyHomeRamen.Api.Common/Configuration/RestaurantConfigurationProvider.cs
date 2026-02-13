using Microsoft.Extensions.Configuration;

namespace MyHomeRamen.Api.Common.Configuration;

public class RestaurantConfigurationProvider(IConfiguration configuration)
{
    private const string SectionKey = "RestaurantConfiguration:";

    public string InfrastructurePrefix => configuration.GetValue<string>($"{SectionKey}InfrastructurePrefix")!;

    public string RestaurantName => configuration[$"{SectionKey}Name"]!;

    public Guid RestaurantId => configuration.GetValue<Guid>($"{SectionKey}RestaurantId");

    public string? IdentityConnectionString => configuration[$"{SectionKey}IdentityConnectionString"];

    public string? MenuConnectionString => configuration[$"{SectionKey}MenuConnectionString"];

    public string? ReservationsConnectionString => configuration[$"{SectionKey}ReservationConnectionString"];

    public string? OrdersConnectionString => configuration[$"{SectionKey}OrderConnectionString"];

    public string? ShoppingCartConnectionString => configuration[$"{SectionKey}ShoppingCartConnectionString"];

    public string? PaymentsConnectionString => configuration[$"{SectionKey}PaymentConnectionString"];

    public string? WorkerConnectionString => configuration[$"{SectionKey}WorkerConnectionString"];
}
