using Microsoft.Extensions.Configuration;
using MyHomeRamen.AppHost.Configurations;
using MyHomeRamen.AppHost.Configurations.Common;

namespace MyHomeRamen.AppHost.InfrastructureConfiguration;

internal static class RegistrationExtensions
{
    private const string ConfigurationSectionPrefix = "InfrastructureConfig:";
    private const string ApplicationNameSetting = "RestaurantConfiguration:InfrastructurePrefix";

    public static IResourceBuilder<RedisResource> ConfigureRedis(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        const string sectionName = $"{ConfigurationSectionPrefix}RedisConfig";

        string prefix = configuration[ApplicationNameSetting] ?? throw new Exception("Application name not configured");
        RedisConfig config = configuration.GetSection(sectionName).Get<RedisConfig>() ?? new();

        IResourceBuilder<ParameterResource> password = builder.AddParameter($"{prefix}-cache-password", config.Password, secret: true);

        return builder.AddRedis(ServiceNames.Cache(prefix), null, password)
                      .WithContainerName($"{prefix}-redis")
                      .WithRedisInsight(config =>
                      {
                          config.WithContainerName($"{prefix}-redis-insight");
                          config.WithExplicitStart();
                      })
                      .WithLifetime(ContainerLifetime.Persistent);
    }

    public static IResourceBuilder<RabbitMQServerResource> ConfigureRabbitMq(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        const string sectionName = $"{ConfigurationSectionPrefix}RabbitMqConfig";

        string prefix = configuration[ApplicationNameSetting] ?? throw new Exception("Application name not configured");
        RabbitMqConfig config = configuration.GetSection(sectionName).Get<RabbitMqConfig>() ?? new();

        IResourceBuilder<ParameterResource> user = builder.AddParameter($"{prefix}-messaging-user-name", config.UserName, secret: true);
        IResourceBuilder<ParameterResource> password = builder.AddParameter($"{prefix}-messaging-password", config.Password, secret: true);

        return builder.AddRabbitMQ(ServiceNames.RabbitMq(prefix), user, password)
                      .WithContainerName(ServiceNames.RabbitMq(prefix))
                      .WithManagementPlugin()
                      .WithOtlpExporter()
                      .WithLifetime(ContainerLifetime.Persistent);
    }

    public static IResourceBuilder<KeycloakResource> ConfigureKeyCloak(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        const string sectionName = $"{ConfigurationSectionPrefix}KeyCloakConfig";

        string prefix = configuration[ApplicationNameSetting] ?? throw new Exception("Application name not configured");
        KeyCloakConfig config = configuration.GetSection(sectionName).Get<KeyCloakConfig>() ?? new();

        IResourceBuilder<ParameterResource> user = builder.AddParameter($"{prefix}-key-cloak-user-name", config.UserName, secret: true);
        IResourceBuilder<ParameterResource> password = builder.AddParameter($"{prefix}-key-cloak-password", config.Password, secret: true);

        return builder.AddKeycloak(ServiceNames.KeyCloak(prefix), 8080, user, password)
                      .WithContainerName(ServiceNames.KeyCloak(prefix))
                      .WithDataVolume("keycloak")
                      .WithRealmImport("./Configurations/Keycloak")
                      .WithBindMount("./Configurations/Keycloak/themes/my-custom-theme", "/opt/keycloak/themes/my-custom-theme")
                      .WithOtlpExporter()
                      .WithLifetime(ContainerLifetime.Persistent);
    }
}
