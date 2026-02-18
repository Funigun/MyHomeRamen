using Microsoft.Extensions.Configuration;

namespace MyHomeRamen.AppHost.Configurations.Common;

internal static class ConfigurationExtensions
{
    internal static string GetModuleConnectionString(this IConfiguration configuration, string moduleName)
    {
        // Configure user secrets usiong following commands before running the application
        // IMPORTANT: CustomConfig:Menu:User and CustomConfig:Menu:Password must be reapeated for all available modules
        //            see ConfigurationConstants
        /*
            dotnet user-secrets set "CustomConfig:ConnectionTemplate" "Server=[Server};Database=[DbName];User Id=[UserName];Password=[Password]"
            dotnet user-secrets set "CustomConfig:Server" "[YOUR_SERVER_ADDRESS]"
            dotnet user-secrets set "CustomConfig:Menu:User" "[Menu_Admin]"
            dotnet user-secrets set "CustomConfig:Menu:Password" "[Menu_Admin_Password]"
        */

        string connectionTemplate = configuration[$"CustomConfig:ConnectionTemplate"]!;
        string server = configuration["CustomConfig:Server"] ?? ".";
        string databaseName = configuration["CustomConfig:DatabaseName"]!;
        string? userName = configuration[$"CustomConfig:{moduleName}:User"];
        string? password = configuration[$"CustomConfig:{moduleName}:Password"];

        string userNamePlaceholder = userName is null ? "User Id=[UserName]" : "[UserName]";
        string passwordPlaceholder = password is null ? ";Password=[Password]" : "[Password]";

        return connectionTemplate?
              .Replace("[Server]", server, StringComparison.InvariantCulture)!
              .Replace("[DbName]", databaseName, StringComparison.InvariantCulture)!
              .Replace(userNamePlaceholder, userName, StringComparison.InvariantCulture)!
              .Replace(passwordPlaceholder, password, StringComparison.InvariantCulture)!;
    }
}
