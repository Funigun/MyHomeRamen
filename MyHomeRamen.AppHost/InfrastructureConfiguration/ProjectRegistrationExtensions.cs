namespace MyHomeRamen.AppHost.InfrastructureConfiguration;

internal static class ProjectRegistrationExtensions
{
    public static IResourceBuilder<ProjectResource> AddApiService(this IDistributedApplicationBuilder builder, string resourcePrefix, IResourceBuilder<RedisResource> cache, IResourceBuilder<RabbitMQServerResource> rabbitmq)
    {
        return builder.AddProject<Projects.MyHomeRamen_Api>($"{resourcePrefix}api")
                      .WithHttpHealthCheck("/health")
                      .WithReference(cache)
                      .WaitFor(cache)
                      .WaitFor(rabbitmq)
                      .WithReference(rabbitmq);
    }

    public static IResourceBuilder<ProjectResource> AddIdentityApiService(this IDistributedApplicationBuilder builder, string resourcePrefix)
    {
        return builder.AddProject<Projects.MyHomeRamen_Identity_Api>($"{resourcePrefix}identity-api")
                      .WithHttpHealthCheck("/health");
    }

    public static IResourceBuilder<ProjectResource> AddBlazor(this IDistributedApplicationBuilder builder, string resourcePrefix, IResourceBuilder<RedisResource> cache, IResourceBuilder<ProjectResource> apiService, IResourceBuilder<ProjectResource> identityApiService)
    {
        return builder.AddProject<Projects.MyHomeRamen_Blazor>($"{resourcePrefix}blazor")
                      .WithExternalHttpEndpoints()
                      .WithHttpHealthCheck("/health")
                      .WithReference(cache)
                      .WaitFor(cache)
                      .WithReference(apiService)
                      .WaitFor(apiService)
                      .WaitFor(identityApiService)
                      .WithReference(identityApiService);
    }

    public static void AddWorkers(this IDistributedApplicationBuilder builder, string resourcePrefix, IResourceBuilder<ProjectResource> apiService)
    {
        builder.AddProject<Projects.MyHomeRamen_Worker_MailSender>($"{resourcePrefix}mailing-worker")
               .WithReference(apiService)
               .WaitFor(apiService)
               .WithExplicitStart();

        builder.AddProject<Projects.MyHomeRamen_Worker_MessagesHandler>($"{resourcePrefix}messages-worker")
               .WithReference(apiService)
               .WaitFor(apiService)
               .WithExplicitStart();
    }
}
