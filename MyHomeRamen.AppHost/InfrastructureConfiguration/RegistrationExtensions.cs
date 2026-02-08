using Microsoft.Extensions.Configuration;
using MyHomeRamen.AppHost.Configurations;

namespace MyHomeRamen.AppHost.InfrastructureConfiguration;

internal static class RegistrationExtensions
{
    private const string ConfigurationSectionPrefix = "InfrastructureConfig:";
    private const string ApplicationNameSetting = "CustomConfig:ApplicationName";

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

        IResourceBuilder<ParameterResource> user = builder.AddParameter($"{applicationName}-key-cloak-user-name", config.UserName, secret: true);
        IResourceBuilder<ParameterResource> password = builder.AddParameter($"{applicationName}-key-cloak-password", config.Password, secret: true);

        return builder.AddKeycloak($"{applicationName}-key-cloak", 8080, user, password)
                      .WithContainerName($"{applicationName}-key-cloak")
                      //.WithBindMount(config.BindMountFrom!, config.BindMountTo!)
                      //.WithBindMount(Path.Combine(AppContext.BaseDirectory, "realm-init-template"), "/opt/keycloak/data/import/realm.json")
                      .WithOtlpExporter()
                      .WithLifetime(ContainerLifetime.Persistent);
    }

    public static IResourceBuilder<PostgresServerResource> ConfigurePostgresDb(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        const string sectionName = $"{ConfigurationSectionPrefix}PostgresConfig";

        string applicationName = configuration[ApplicationNameSetting] ?? throw new Exception("Application name not configured");
        PostgresConfig config = configuration.GetSection(sectionName).Get<PostgresConfig>() ?? new();

        IResourceBuilder<ParameterResource> user = builder.AddParameter($"{applicationName}-postgres-db-user-name", config.UserName, secret: true);
        IResourceBuilder<ParameterResource> password = builder.AddParameter($"{applicationName}-postgres-db-password", config.Password, secret: true);

        return builder.AddPostgres($"{applicationName}-postgres-db", user, password)
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
