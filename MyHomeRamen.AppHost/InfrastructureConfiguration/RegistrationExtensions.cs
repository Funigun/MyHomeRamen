using Microsoft.Extensions.Configuration;

namespace MyHomeRamen.AppHost.InfrastructureConfiguration;

internal static class RegistrationExtensions
{
    public static IResourceBuilder<RedisResource> AddRedis(this IDistributedApplicationBuilder builder, string resourcePrefix, IResourceBuilder<ParameterResource>? password)
    {
        return builder.AddRedis($"{resourcePrefix}cache", null, password)
                      .WithRedisInsight()
                      .WithLifetime(ContainerLifetime.Persistent);
    }

    public static IResourceBuilder<RabbitMQServerResource> AddRabbitMq(this IDistributedApplicationBuilder builder, string resourcePrefix, IResourceBuilder<ParameterResource>? username, IResourceBuilder<ParameterResource>? password)
    {
        return builder.AddRabbitMQ($"{resourcePrefix}messaging", username, password)
                      .WithManagementPlugin()
                      .WithLifetime(ContainerLifetime.Persistent);
    }

    public static IResourceBuilder<ContainerResource> AddSeq(this IDistributedApplicationBuilder builder, IConfiguration config, IResourceBuilder<ProjectResource> apiService)
    {
        return builder.AddContainer("seq", "datalust/seq")
                      .WithContainerName("seq-aspire")
                      .WithEnvironment("ACCEPT_EULA", "Y")
                      .WithBindMount(config["InfrastructureConfig:Seq:BindMountFrom"]!, config["InfrastructureConfig:Seq:BindMountTo"]!)
                      .WithHttpEndpoint(8081, 80)
                      .WithHttpEndpoint(5341, 5341)
                      .WithReference(apiService)
                      .WithLifetime(ContainerLifetime.Persistent);
    }

    public static IResourceBuilder<ContainerResource> AddJaeger(this IDistributedApplicationBuilder builder)
    {
        return builder.AddContainer("jaeger", "jaegertracing/all-in-one")
                      .WithContainerName("jaeger-aspire")
                      .WithHttpEndpoint(16686, targetPort: 16686, name: "jaegerPortal")
                      .WithHttpEndpoint(4317, targetPort: 4317, name: "jaegerEndpoint")
                      .WithLifetime(ContainerLifetime.Persistent);
    }
}
