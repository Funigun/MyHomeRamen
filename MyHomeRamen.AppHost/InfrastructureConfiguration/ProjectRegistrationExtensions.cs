using Microsoft.Extensions.Configuration;
using MyHomeRamen.AppHost.Configurations.Common;

namespace MyHomeRamen.AppHost.InfrastructureConfiguration;

internal static class ProjectRegistrationExtensions
{
    private const string ConfigurationSectionPrefix = "RestaurantConfiguration:InfrastructurePrefix";
    private const string RealmKey = "Authorization:Realm";
    private const string AudienceKey = "Authorization:Audience";
    private const string AuthBaseUrlKey = "Authorization:BaseUrl";
    private const string AdminApiClientIdKey = "Authentication:KeycloakAdmin:ClientId";
    private const string AdminApiClientSecretKey = "Authentication:KeycloakAdmin:ClientSecret";

    public static IResourceBuilder<ProjectResource> AddApiService(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        string applicationName = configuration[ConfigurationSectionPrefix] ?? throw new Exception("Application name not configured");

        IEnumerable<string> requiredModules = [
            ConfigurationConstants.MenuModuleName,
            ConfigurationConstants.ReservationModuleName,
            ConfigurationConstants.OrderModuleName,
            ConfigurationConstants.ShoppingCartModuleName,
            ConfigurationConstants.PaymentModuleName
        ];

        return builder.AddProject<Projects.MyHomeRamen_Api>($"{applicationName}-api")
                      .WithModulesAccess(requiredModules, configuration)
                      .WithRestaurantConfig(configuration)
                      .WithHttpHealthCheck("/health");
    }

    public static IResourceBuilder<ProjectResource> AddIdentityApiService(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        string applicationName = configuration[ConfigurationSectionPrefix] ?? throw new Exception("Application name not configured");

        IEnumerable<string> requiredModules = [
            ConfigurationConstants.IdentityModuleName
        ];

        return builder.AddProject<Projects.MyHomeRamen_Identity_Api>($"{applicationName}-identity-api")
                      .WithModulesAccess(requiredModules, configuration)
                      .WithRestaurantConfig(configuration)
                      .WithEnvironment(RealmKey, configuration[RealmKey])
                      .WithEnvironment(AudienceKey, configuration[AudienceKey])
                      .WithEnvironment(AuthBaseUrlKey, configuration[AuthBaseUrlKey])
                      .WithEnvironment(AdminApiClientIdKey, configuration[AdminApiClientIdKey])
                      .WithEnvironment(AdminApiClientSecretKey, configuration[AdminApiClientSecretKey])
                      .WithHttpHealthCheck("/health");
    }

    public static IResourceBuilder<ProjectResource> AddBlazor(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        string applicationName = configuration[ConfigurationSectionPrefix] ?? throw new Exception("Application name not configured");

        return builder.AddProject<Projects.MyHomeRamen_Blazor>($"{applicationName}-blazor")
                      .WithExternalHttpEndpoints()
                      .WithRestaurantConfig(configuration)
                      .WithEnvironment("Authentication:Blazor:ClientId", configuration["Authentication:Blazor:ClientId"])
                      .WithEnvironment("Authentication:Blazor:ClientSecret", configuration["Authentication:Blazor:ClientSecret"])
                      .WithEnvironment(RealmKey, configuration[RealmKey])
                      .WithHttpHealthCheck("/health");
    }

    public static IResourceBuilder<ProjectResource> AddDbinitializer(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        string applicationName = configuration[ConfigurationSectionPrefix] ?? throw new Exception("Application name not configured");

        IEnumerable<string> requiredModules = [
            ConfigurationConstants.MenuModuleName,
            ConfigurationConstants.ReservationModuleName,
            ConfigurationConstants.OrderModuleName,
            ConfigurationConstants.ShoppingCartModuleName,
            ConfigurationConstants.PaymentModuleName,
            ConfigurationConstants.IdentityModuleName
        ];

        return builder.AddProject<Projects.MyHomeRamen_Worker_DatabaseInitializer>($"{applicationName}-db-initializer")
                      .WithModulesAccess(requiredModules, configuration)
                      .WithRestaurantConfig(configuration)
                      .WithUsersConfiguration(requiredModules, configuration);
    }

    public static void AddWorkers(this IDistributedApplicationBuilder builder, IConfiguration configuration, IResourceBuilder<ProjectResource> apiService)
    {
        string applicationName = configuration[ConfigurationSectionPrefix] ?? throw new Exception("Application name not configured");

        builder.AddProject<Projects.MyHomeRamen_Worker_MailSender>($"{applicationName}-mailing-worker")
               .WithReference(apiService)
               .WaitFor(apiService)
               .WithRestaurantConfig(configuration)
               .WithExplicitStart();

        builder.AddProject<Projects.MyHomeRamen_Worker_MessagesHandler>($"{applicationName}-messages-worker")
               .WithReference(apiService)
               .WaitFor(apiService)
               .WithRestaurantConfig(configuration)
               .WithExplicitStart();
    }
}
