using Microsoft.Extensions.Configuration;

namespace MyHomeRamen.AppHost.Configurations.Common;

internal static class ConfigurationExtensions
{
    internal static string GetModuleConnectionString(this IConfiguration configuration, string moduleName)
    {
        string connectionTemplate = configuration[$"{ConfigurationConstants.DatabaseConfigurationSection}:ConnectionTemplate"]!;
        string server = configuration[$"{ConfigurationConstants.DatabaseConfigurationSection}:Server"] ?? ".";
        string databaseName = configuration[$"{ConfigurationConstants.DatabaseConfigurationSection}:DatabaseName"]!;
        string? userName = configuration[$"{ConfigurationConstants.DatabaseConfigurationSection}:{moduleName}:User"];
        string? password = configuration[$"{ConfigurationConstants.DatabaseConfigurationSection}:{moduleName}:Password"];

        string userNamePlaceholder = userName is null ? "User Id=[UserName]" : "[UserName]";
        string passwordPlaceholder = password is null ? ";Password=[Password]" : "[Password]";

        return connectionTemplate?
              .Replace("[Server]", server, StringComparison.InvariantCulture)!
              .Replace("[DbName]", databaseName, StringComparison.InvariantCulture)!
              .Replace(userNamePlaceholder, userName, StringComparison.InvariantCulture)!
              .Replace(passwordPlaceholder, password, StringComparison.InvariantCulture)!;
    }

    internal static IResourceBuilder<ProjectResource> WithModulesAccess(this IResourceBuilder<ProjectResource> builder, IEnumerable<string> modules, IConfiguration configuration)
    {
        foreach (string module in modules)
        {
            builder.WithEnvironment($"ConnectionStrings__{module}", configuration.GetModuleConnectionString(module))
                   .WithEnvironment($"{ConfigurationConstants.RestaurantConfigurationSection}__{module}ConnectionString", $"ConnectionStrings:{module}");
        }

        return builder;
    }

    internal static IResourceBuilder<ProjectResource> WithUsersConfiguration(this IResourceBuilder<ProjectResource> builder, IEnumerable<string> modules, IConfiguration configuration)
    {
        foreach (string module in modules)
        {
            builder.WithEnvironment($"{ConfigurationConstants.DatabaseConfigurationSection}__{module}__User", configuration[$"{ConfigurationConstants.DatabaseConfigurationSection}:{module}:User"]!)
                   .WithEnvironment($"{ConfigurationConstants.DatabaseConfigurationSection}__{module}__Password", configuration[$"{ConfigurationConstants.DatabaseConfigurationSection}:{module}:Password"]!);
        }

        return builder;
    }

    internal static IResourceBuilder<ProjectResource> WithRestaurantConfig(this IResourceBuilder<ProjectResource> builder, IConfiguration configuration)
    {
        string restaurantId = configuration[$"{ConfigurationConstants.RestaurantConfigurationSection}:RestaurantId"]!;
        string restaurantName = configuration[$"{ConfigurationConstants.RestaurantConfigurationSection}:Name"]!;
        string infrastructurePrefix = configuration[$"{ConfigurationConstants.RestaurantConfigurationSection}:InfrastructurePrefix"]!;

        return builder.WithEnvironment($"{ConfigurationConstants.RestaurantConfigurationSection}__RestaurantId", restaurantId)
                      .WithEnvironment($"{ConfigurationConstants.RestaurantConfigurationSection}__Name", restaurantName)
                      .WithEnvironment($"{ConfigurationConstants.RestaurantConfigurationSection}__InfrastructurePrefix", infrastructurePrefix);
    }
}
