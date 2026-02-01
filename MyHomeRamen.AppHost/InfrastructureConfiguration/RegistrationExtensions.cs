using Microsoft.Extensions.Configuration;

namespace MyHomeRamen.AppHost.InfrastructureConfiguration;

internal static class RegistrationExtensions
{
    public static IResourceBuilder<RedisResource> AddRedis(this IDistributedApplicationBuilder builder, string resourcePrefix, IResourceBuilder<ParameterResource>? password)
    {
        return builder.AddRedis($"{resourcePrefix}cache", null, password)
                      .WithContainerName("my-home-ramen-redis")
                      .WithRedisInsight()
                      .WithLifetime(ContainerLifetime.Persistent);
    }

    public static IResourceBuilder<RabbitMQServerResource> AddRabbitMq(this IDistributedApplicationBuilder builder, string resourcePrefix, IResourceBuilder<ParameterResource>? username, IResourceBuilder<ParameterResource>? password)
    {
        return builder.AddRabbitMQ($"{resourcePrefix}messaging", username, password)
                      .WithContainerName("my-home-ramen-rabbitmq")
                      .WithManagementPlugin()
                      .WithLifetime(ContainerLifetime.Persistent);
    }

    public static IResourceBuilder<ContainerResource> AddSeq(this IDistributedApplicationBuilder builder, IConfiguration config, IResourceBuilder<ProjectResource> apiService)
    {
        return builder.AddContainer("seq", "datalust/seq")
                      .WithContainerName("my-home-ramen-seq")
                      .WithEnvironment("ACCEPT_EULA", "Y")
                      //.WithBindMount(config["InfrastructureConfig:Seq:BindMountFrom"]!, config["InfrastructureConfig:Seq:BindMountTo"]!)
                      .WithHttpEndpoint(8081, 80, "main")
                      .WithHttpEndpoint(5341, 5341, "other")
                      .WithReference(apiService)
                      .WithLifetime(ContainerLifetime.Persistent);
    }

    public static IResourceBuilder<ContainerResource> AddJaeger(this IDistributedApplicationBuilder builder)
    {
        return builder.AddContainer("jaeger", "jaegertracing/all-in-one")
                      .WithContainerName("my-home-ramen-jaeger")
                      //.WithBindMount(config["InfrastructureConfig:Jaeger:BindMountFrom"]!, config["InfrastructureConfig:Jaeger:BindMountTo"]!)
                      .WithHttpEndpoint(16686, targetPort: 16686, name: "jaegerPortal")
                      .WithHttpEndpoint(4317, targetPort: 4317, name: "jaegerEndpoint")
                      .WithLifetime(ContainerLifetime.Persistent);
    }
}
