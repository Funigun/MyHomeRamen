using Microsoft.Extensions.Configuration;
using MyHomeRamen.AppHost.Configurations;

namespace MyHomeRamen.AppHost.InfrastructureConfiguration;

internal static class RegistrationExtensions
{
    private const string ConfigurationSectionPrefix = "InfrastructureConfig:";
    private const string ApplicationNameSetting = "CustomConfig:ApplicationName";
    private static readonly GenerateParameterDefault _defaultParameterOptions = new GenerateParameterDefault { MinLength = 22, Special = true };

    public static IResourceBuilder<RedisResource> ConfigureRedis(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        const string sectionName = $"{ConfigurationSectionPrefix}RedisConfig";

        string applicationName = configuration[ApplicationNameSetting] ?? throw new Exception("Application name not configured");
        RedisConfig config = configuration.GetSection(sectionName).Get<RedisConfig>() ?? new();

        IResourceBuilder<ParameterResource> password = builder.AddParameter($"{applicationName}-cache-password", _defaultParameterOptions, secret: true);

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

        IResourceBuilder<ParameterResource> user = builder.AddParameter($"{applicationName}-messaging-user-name", _defaultParameterOptions, secret: true);
        IResourceBuilder<ParameterResource> password = builder.AddParameter($"{applicationName}-messaging-password", _defaultParameterOptions, secret: true);

        return builder.AddRabbitMQ($"{applicationName}-messaging", user, password)
                      .WithContainerName($"{applicationName}-rabbitmq")
                      //.WithBindMount(config.BindMountFrom!, config.BindMountTo!)
                      .WithManagementPlugin()
                      .WithLifetime(ContainerLifetime.Persistent);
    }

    public static IResourceBuilder<ContainerResource> ConfigureSeq(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        const string sectionName = $"{ConfigurationSectionPrefix}SeqConfig";

        string applicationName = configuration[ApplicationNameSetting] ?? throw new Exception("Application name not configured");
        SeqConfig config = configuration.GetSection(sectionName).Get<SeqConfig>() ?? new();

        return builder.AddContainer($"{applicationName}-seq", "datalust/seq")
                      .WithContainerName($"{applicationName}-seq")
                      .WithEnvironment("ACCEPT_EULA", "Y")
                      //.WithBindMount(config.BindMountFrom!, config.BindMountTo!)
                      .WithHttpEndpoint(8081, 80, "main")
                      .WithHttpEndpoint(5341, 5341, "other")
                      .WithLifetime(ContainerLifetime.Persistent);
    }

    public static IResourceBuilder<ContainerResource> ConfigureJaeger(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        const string sectionName = $"{ConfigurationSectionPrefix}JaegerConfig";

        string applicationName = configuration[ApplicationNameSetting] ?? throw new Exception("Application name not configured");
        JaegerConfig config = configuration.GetSection(sectionName).Get<JaegerConfig>() ?? new();

        return builder.AddContainer($"{applicationName}-jaeger", "jaegertracing/all-in-one")
                      .WithContainerName($"{applicationName}-jaeger")
                      //.WithBindMount(config.BindMountFrom!, config.BindMountTo!)
                      .WithHttpEndpoint(16686, targetPort: 16686, name: "jaegerPortal")
                      .WithHttpEndpoint(4317, targetPort: 4317, name: "jaegerEndpoint")
                      .WithLifetime(ContainerLifetime.Persistent);
    }

    public static IResourceBuilder<KeycloakResource> ConfigureKeyCloak(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        const string sectionName = $"{ConfigurationSectionPrefix}KeyCloakConfig";

        string applicationName = configuration[ApplicationNameSetting] ?? throw new Exception("Application name not configured");
        KeyCloakConfig config = configuration.GetSection(sectionName).Get<KeyCloakConfig>() ?? new();

        return builder.AddKeycloak($"{applicationName}-key-cloak", 8080)
                      .WithContainerName($"{applicationName}-key-cloak")
                      //.WithBindMount(config.BindMountFrom!, config.BindMountTo!)
                      .WithOtlpExporter()
                      .WithLifetime(ContainerLifetime.Persistent);
    }

    public static IResourceBuilder<PostgresServerResource> ConfigurePostgresDb(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        const string sectionName = $"{ConfigurationSectionPrefix}KeyCloakConfig";

        string applicationName = configuration[ApplicationNameSetting] ?? throw new Exception("Application name not configured");
        PostgresConfig config = configuration.GetSection(sectionName).Get<PostgresConfig>() ?? new();

        return builder.AddPostgres($"{applicationName}-postgres-db")
                      .WithContainerName($"{applicationName}-postgres")
                      //.WithBindMount(config.BindMountFrom!, config.BindMountTo!)
                      .WithEnvironment("ACCEPT_EULA", "Y")
                      .WithPgWeb(config =>
                      {
                          config.WithContainerName($"{applicationName}-postgres-web-view");
                          config.WithExplicitStart();
                      })
                      .WithLifetime(ContainerLifetime.Persistent);
    }
}
