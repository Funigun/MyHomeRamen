using Microsoft.Extensions.Configuration;

namespace MyHomeRamen.AppHost.InfrastructureConfiguration;

internal static class ProjectRegistrationExtensions
{
    private const string ApplicationNameSetting = "CustomConfig:ApplicationName";

    public static IResourceBuilder<ProjectResource> AddApiService(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        string applicationName = configuration[ApplicationNameSetting] ?? throw new Exception("Application name not configured");

        return builder.AddProject<Projects.MyHomeRamen_Api>($"{applicationName}-api")
                      .WithHttpHealthCheck("/health");
    }

    public static IResourceBuilder<ProjectResource> AddIdentityApiService(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        string applicationName = configuration[ApplicationNameSetting] ?? throw new Exception("Application name not configured");

        return builder.AddProject<Projects.MyHomeRamen_Identity_Api>($"{applicationName}-identity-api")
                      .WithHttpHealthCheck("/health");
    }

    public static IResourceBuilder<ProjectResource> AddBlazor(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        string applicationName = configuration[ApplicationNameSetting] ?? throw new Exception("Application name not configured");

        return builder.AddProject<Projects.MyHomeRamen_Blazor>($"{applicationName}-blazor")
                      .WithExternalHttpEndpoints()
                      .WithHttpHealthCheck("/health");
    }

    public static void AddWorkers(this IDistributedApplicationBuilder builder, IConfiguration configuration, IResourceBuilder<ProjectResource> apiService)
    {
        string applicationName = configuration[ApplicationNameSetting] ?? throw new Exception("Application name not configured");

        builder.AddProject<Projects.MyHomeRamen_Worker_MailSender>($"{applicationName}-mailing-worker")
               .WithReference(apiService)
               .WaitFor(apiService)
               .WithExplicitStart();

        builder.AddProject<Projects.MyHomeRamen_Worker_MessagesHandler>($"{applicationName}-messages-worker")
               .WithReference(apiService)
               .WaitFor(apiService)
               .WithExplicitStart();
    }
}
