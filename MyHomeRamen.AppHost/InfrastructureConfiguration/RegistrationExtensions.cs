using Microsoft.Extensions.Configuration;
using MyHomeRamen.AppHost.Configurations;

namespace MyHomeRamen.AppHost.InfrastructureConfiguration;

internal static class RegistrationExtensions
{
    private const string ConfigurationSectionPrefix = "InfrastructureConfig:";
    private const string ApplicationNameSetting = "RestaurantConfiguration:InfrastructurePrefix";

    public static IResourceBuilder<RedisResource> ConfigureRedis(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        const string sectionName = $"{ConfigurationSectionPrefix}RedisConfig";

        string applicationName = configuration[ApplicationNameSetting] ?? throw new Exception("Application name not configured");
        RedisConfig config = configuration.GetSection(sectionName).Get<RedisConfig>() ?? new();

        IResourceBuilder<ParameterResource> password = builder.AddParameter($"{applicationName}-cache-password", config.Password, secret: true);

        return builder.AddRedis($"{applicationName}-cache", null, password)
                      .WithContainerName($"{applicationName}-redis")
                      //.WithBindMount(config.BindMountFrom!, config.BindMountTo!)
                      .WithRedisInsight(config =>
                      {
                          config.WithContainerName($"{applicationName}-redis-insight");
                          config.WithExplicitStart();
                      })
                      .WithLifetime(ContainerLifetime.Persistent);
    }

    public static IResourceBuilder<RabbitMQServerResource> ConfigureRabbitMq(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        const string sectionName = $"{ConfigurationSectionPrefix}RabbitMqConfig";

        string applicationName = configuration[ApplicationNameSetting] ?? throw new Exception("Application name not configured");
        RabbitMqConfig config = configuration.GetSection(sectionName).Get<RabbitMqConfig>() ?? new();

        IResourceBuilder<ParameterResource> user = builder.AddParameter($"{applicationName}-messaging-user-name", config.UserName, secret: true);
        IResourceBuilder<ParameterResource> password = builder.AddParameter($"{applicationName}-messaging-password", config.Password, secret: true);

        return builder.AddRabbitMQ($"{applicationName}-messaging", user, password)
                      .WithContainerName($"{applicationName}-rabbitmq")
                      //.WithBindMount(config.BindMountFrom!, config.BindMountTo!)
                      .WithManagementPlugin()
                      .WithOtlpExporter()
                      .WithLifetime(ContainerLifetime.Persistent);
    }

    public static IResourceBuilder<KeycloakResource> ConfigureKeyCloak(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        const string sectionName = $"{ConfigurationSectionPrefix}KeyCloakConfig";

        string applicationName = configuration[ApplicationNameSetting] ?? throw new Exception("Application name not configured");
        KeyCloakConfig config = configuration.GetSection(sectionName).Get<KeyCloakConfig>() ?? new();

        IResourceBuilder<ParameterResource> user = builder.AddParameter($"{applicationName}-key-cloak-user-name", config.UserName, secret: true);
        IResourceBuilder<ParameterResource> password = builder.AddParameter($"{applicationName}-key-cloak-password", config.Password, secret: true);

        return builder.AddKeycloak($"{applicationName}-key-cloak", 8080, user, password)
                      .WithContainerName($"{applicationName}-key-cloak")
                      .WithDataVolume("keycloak")
                      .WithRealmImport("./Configurations/Keycloak")
                      .WithOtlpExporter()
                      .WithLifetime(ContainerLifetime.Persistent);
    }
}
