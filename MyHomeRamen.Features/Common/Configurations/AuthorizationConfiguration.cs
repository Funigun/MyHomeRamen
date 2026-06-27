using Microsoft.Extensions.Configuration;

namespace MyHomeRamen.Features.Common.Configurations;

public sealed class AuthorizationConfiguration(IConfiguration configuration)
{
    public string Realm { get; } = configuration["Authorization:Realm"]!;

    public string Audience { get; } = configuration["Authorization:Audience"]!;

    public string BaseUrl { get; } = configuration[$"services:{configuration["RestaurantConfiguration:InfrastructurePrefix"]!}-key-cloak:https:0"]!;

    public string Authority => $"{BaseUrl.TrimEnd("/")}/realms/{Realm}";

    public string Issuer => $"{BaseUrl.TrimEnd("/")}/realms/{Realm}";
}
