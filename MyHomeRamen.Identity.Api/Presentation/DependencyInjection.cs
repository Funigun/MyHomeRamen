using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Identity.Api.Infrastructure;
using Scalar.AspNetCore;

namespace MyHomeRamen.Identity.Api.Presentation;

internal static class DependencyInjection
{
    internal const string RestaurantPolicy = "RestaurantPolicy";
    internal const string AdminPolicy = "AdminPolicy";
    internal const string KeycloakBearerScheme = "KeycloakBearer";

    internal static ScalarOptions ConfigureScalarOptions(this ScalarOptions options, RestaurantConfigurationProvider configurationProvider)
    {
        options.WithTitle(configurationProvider.RestaurantName)
               .WithTheme(ScalarTheme.Kepler)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

        options.AddPreferredSecuritySchemes("Bearer");
        options.AddHttpAuthentication("Bearer", o => o.Description = "Provide valid token");

        options.Servers = [new("https://localhost:7188")];

        return options;
    }

    internal static IServiceCollection ConfigureAuthorizationPolicies(this IServiceCollection services, string policy)
    {
        services.AddAuthorizationBuilder()
                .AddPolicy(policy, policy => policy.RequireAuthenticatedUser())
                .AddPolicy(AdminPolicy, policy =>
                    policy.AddAuthenticationSchemes(KeycloakBearerScheme)
                          .RequireAuthenticatedUser());

        return services;
    }

    internal static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        string keycloakAuthority = ResolveKeycloakAuthority(configuration);
        string[] validIssuers = ResolveValidIssuers(configuration, keycloakAuthority);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = keycloakAuthority;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuers = validIssuers,
                        ValidAudiences = [configuration["Authorization:Audience"]!, "account"],
                        NameClaimType = "preferred_username",
                        RoleClaimType = ClaimTypes.Role,
                    };
                })
                .AddJwtBearer(KeycloakBearerScheme, options =>
                {
                    options.Authority = keycloakAuthority;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuers = validIssuers,
                        ValidAudiences = [configuration["Authorization:Audience"]!, "account"],
                        NameClaimType = "preferred_username",
                        RoleClaimType = ClaimTypes.Role,
                    };
                });

        services.AddTransient<IClaimsTransformation, KeycloakRolesClaimsTransformation>();

        return services;
    }

    private static string ResolveKeycloakAuthority(IConfiguration configuration)
    {
        string prefix = configuration["RestaurantConfiguration:InfrastructurePrefix"]!;

        string keycloakBaseUrl =
            configuration[$"services:{prefix}-key-cloak:https:0"] ??
            configuration.GetConnectionString($"{prefix}-key-cloak") ??
            configuration["Authorization:BaseUrl"] ??
            throw new InvalidOperationException(
                "Keycloak base URL is not configured. Ensure the Aspire reference or 'Authorization:BaseUrl' is set.");

        string realm = configuration["Authorization:Realm"]
            ?? throw new InvalidOperationException("'Authorization:Realm' is not configured.");

        return $"{keycloakBaseUrl.TrimEnd('/')}/realms/{realm}";
    }

    private static string[] ResolveValidIssuers(IConfiguration configuration, string httpAuthority)
    {
        string prefix = configuration["RestaurantConfiguration:InfrastructurePrefix"]!;

        List<string> issuers = [httpAuthority];

        string? httpsBaseUrl = configuration[$"services:{prefix}-key-cloak:https:0"];
        string? realm = configuration["Authorization:Realm"];

        if (!string.IsNullOrEmpty(httpsBaseUrl) && !string.IsNullOrEmpty(realm))
        {
            issuers.Add($"{httpsBaseUrl.TrimEnd('/')}/realms/{realm}");
        }

        return [.. issuers];
    }
}
