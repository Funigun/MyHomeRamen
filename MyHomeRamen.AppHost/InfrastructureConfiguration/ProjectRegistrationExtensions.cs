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
        string prefix = configuration[ConfigurationSectionPrefix] ?? throw new Exception("Application name not configured");

        IEnumerable<string> requiredModules = [
            ConfigurationConstants.MenuModuleName,
            ConfigurationConstants.ReservationModuleName,
            ConfigurationConstants.OrderModuleName,
            ConfigurationConstants.ShoppingCartModuleName,
            ConfigurationConstants.PaymentModuleName
        ];

        return builder.AddProject<Projects.MyHomeRamen_Api>(ServiceNames.Api(prefix))
                      .WithModulesAccess(requiredModules, configuration)
                      .WithRestaurantConfig(configuration)
                      .WithEnvironment(RealmKey, configuration[RealmKey])
                      .WithEnvironment(AudienceKey, configuration[AudienceKey])
                      .WithEnvironment(AuthBaseUrlKey, configuration[AuthBaseUrlKey])
                      .WithHttpHealthCheck("/health");
    }

    public static IResourceBuilder<ProjectResource> AddIdentityApiService(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        string prefix = configuration[ConfigurationSectionPrefix] ?? throw new Exception("Application name not configured");

        IEnumerable<string> requiredModules = [
            ConfigurationConstants.IdentityModuleName
        ];

        return builder.AddProject<Projects.MyHomeRamen_Identity_Api>(ServiceNames.IdentityApi(prefix))
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
        string prefix = configuration[ConfigurationSectionPrefix] ?? throw new Exception("Application name not configured");

        return builder.AddProject<Projects.MyHomeRamen_Blazor>(ServiceNames.Blazor(prefix))
                      .WithExternalHttpEndpoints()
                      .WithRestaurantConfig(configuration)
                      .WithEnvironment("Authentication:Blazor:ClientId", configuration["Authentication:Blazor:ClientId"])
                      .WithEnvironment("Authentication:Blazor:ClientSecret", configuration["Authentication:Blazor:ClientSecret"])
                      .WithEnvironment(RealmKey, configuration[RealmKey])
                      .WithHttpHealthCheck("/health");
    }

    public static IResourceBuilder<ProjectResource> AddDbinitializer(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        string prefix = configuration[ConfigurationSectionPrefix] ?? throw new Exception("Application name not configured");

        IEnumerable<string> requiredModules = [
            ConfigurationConstants.MenuModuleName,
            ConfigurationConstants.ReservationModuleName,
            ConfigurationConstants.OrderModuleName,
            ConfigurationConstants.ShoppingCartModuleName,
            ConfigurationConstants.PaymentModuleName,
            ConfigurationConstants.IdentityModuleName
        ];

        return builder.AddProject<Projects.MyHomeRamen_Worker_DatabaseInitializer>(ServiceNames.DbInitializerWorker(prefix))
                      .WithModulesAccess(requiredModules, configuration)
                      .WithRestaurantConfig(configuration)
                      .WithUsersConfiguration(requiredModules, configuration);
    }

    public static IResourceBuilder<ProjectResource> AddMessagesHandlerWorker(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        string prefix = configuration[ConfigurationSectionPrefix] ?? throw new Exception("Application name not configured");

        IEnumerable<string> requiredModules = [
            ConfigurationConstants.MenuModuleName,
            ConfigurationConstants.ReservationModuleName,
            ConfigurationConstants.OrderModuleName,
            ConfigurationConstants.ShoppingCartModuleName,
            ConfigurationConstants.PaymentModuleName,
            ConfigurationConstants.IdentityModuleName
        ];

        return builder.AddProject<Projects.MyHomeRamen_Worker_MessagesHandler>(ServiceNames.MessagesWorker(prefix))
                      .WithRestaurantConfig(configuration)
                      .WithModulesAccess(requiredModules, configuration);
    }

    public static IResourceBuilder<ProjectResource> AddMailingWorker(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        string prefix = configuration[ConfigurationSectionPrefix] ?? throw new Exception("Application name not configured");

        return builder.AddProject<Projects.MyHomeRamen_Worker_MailSender>(ServiceNames.MailingWorker(prefix))
                      .WithRestaurantConfig(configuration)
                      .WithExplicitStart();
    }
}
