using Microsoft.Extensions.Configuration;
using MyHomeRamen.AppHost.Configurations.Common;

namespace MyHomeRamen.AppHost.InfrastructureConfiguration;

internal static class ProjectRegistrationExtensions
{
    public static IResourceBuilder<ProjectResource> AddApiService(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        string applicationName = configuration[ConfigurationConstants.ApplicationNameSetting] ?? throw new Exception("Application name not configured");

        return builder.AddProject<Projects.MyHomeRamen_Api>($"{applicationName}-api")
                      .WithEnvironment("ConnectionStrings__Menu", configuration.GetModuleConnectionString(ConfigurationConstants.MenuModuleName))
                      .WithEnvironment("ConnectionStrings__Reservation", configuration.GetModuleConnectionString(ConfigurationConstants.ReservationModuleName))
                      .WithEnvironment("ConnectionStrings__Order", configuration.GetModuleConnectionString(ConfigurationConstants.OrderModuleName))
                      .WithEnvironment("ConnectionStrings__ShoppingCart", configuration.GetModuleConnectionString(ConfigurationConstants.ShoppingCartModuleName))
                      .WithEnvironment("ConnectionStrings__Payment", configuration.GetModuleConnectionString(ConfigurationConstants.PaymentModuleName))
                      .WithEnvironment("RestaurantConfiguration__MenuConnectionString", "ConnectionStrings:Menu")
                      .WithEnvironment("RestaurantConfiguration__ReservationConnectionString", "ConnectionStrings:Reservation")
                      .WithEnvironment("RestaurantConfiguration__OrderConnectionString", "ConnectionStrings:Order")
                      .WithEnvironment("RestaurantConfiguration__ShoppingCartConnectionString", "ConnectionStrings:ShoppingCart")
                      .WithEnvironment("RestaurantConfiguration__PaymentConnectionString", "ConnectionStrings:Payment")
                      .WithHttpHealthCheck("/health");
    }

    public static IResourceBuilder<ProjectResource> AddIdentityApiService(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        string applicationName = configuration[ConfigurationConstants.ApplicationNameSetting] ?? throw new Exception("Application name not configured");

        return builder.AddProject<Projects.MyHomeRamen_Identity_Api>($"{applicationName}-identity-api")
                      .WithEnvironment("ConnectionStrings__Identity", configuration.GetModuleConnectionString(ConfigurationConstants.IdentityModuleName))
                      .WithEnvironment("RestaurantConfiguration__IdentityConnectionString", "ConnectionStrings:Identity")
                      .WithHttpHealthCheck("/health");
    }

    public static IResourceBuilder<ProjectResource> AddBlazor(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        string applicationName = configuration[ConfigurationConstants.ApplicationNameSetting] ?? throw new Exception("Application name not configured");

        return builder.AddProject<Projects.MyHomeRamen_Blazor>($"{applicationName}-blazor")
                      .WithExternalHttpEndpoints()
                      .WithHttpHealthCheck("/health");
    }

    public static IResourceBuilder<ProjectResource> AddDbinitializer(this IDistributedApplicationBuilder builder, IConfiguration configuration)
    {
        string applicationName = configuration[ConfigurationConstants.ApplicationNameSetting] ?? throw new Exception("Application name not configured");

        return builder.AddProject<Projects.MyHomeRamen_Worker_DatabaseInitializer>($"{applicationName}-db-initializer")
                      .WithEnvironment("ConnectionStrings__Menu", configuration.GetModuleConnectionString(ConfigurationConstants.DbInitializerWorkerName))
                      .WithEnvironment("ConnectionStrings__Reservation", configuration.GetModuleConnectionString(ConfigurationConstants.DbInitializerWorkerName))
                      .WithEnvironment("ConnectionStrings__Order", configuration.GetModuleConnectionString(ConfigurationConstants.DbInitializerWorkerName))
                      .WithEnvironment("ConnectionStrings__ShoppingCart", configuration.GetModuleConnectionString(ConfigurationConstants.DbInitializerWorkerName))
                      .WithEnvironment("ConnectionStrings__Payment", configuration.GetModuleConnectionString(ConfigurationConstants.DbInitializerWorkerName))
                      .WithEnvironment("ConnectionStrings__Identity", configuration.GetModuleConnectionString(ConfigurationConstants.DbInitializerWorkerName))
                      .WithEnvironment("RestaurantConfiguration__MenuConnectionString", "ConnectionStrings:Menu")
                      .WithEnvironment("RestaurantConfiguration__ReservationConnectionString", "ConnectionStrings:Reservation")
                      .WithEnvironment("RestaurantConfiguration__OrderConnectionString", "ConnectionStrings:Order")
                      .WithEnvironment("RestaurantConfiguration__ShoppingCartConnectionString", "ConnectionStrings:ShoppingCart")
                      .WithEnvironment("RestaurantConfiguration__PaymentConnectionString", "ConnectionStrings:Payment")
                      .WithEnvironment("RestaurantConfiguration__IdentityConnectionString", "ConnectionStrings:Identity");
    }

    public static void AddWorkers(this IDistributedApplicationBuilder builder, IConfiguration configuration, IResourceBuilder<ProjectResource> apiService)
    {
        string applicationName = configuration[ConfigurationConstants.ApplicationNameSetting] ?? throw new Exception("Application name not configured");

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
