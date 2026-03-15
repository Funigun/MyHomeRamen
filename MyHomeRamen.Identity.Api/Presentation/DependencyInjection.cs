using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyHomeRamen.Api.Common.Configuration;
using Scalar.AspNetCore;

namespace MyHomeRamen.Identity.Api.Presentation;

internal static class DependencyInjection
{
    internal const string RestaurantCustomerPolicy = "RestaurantCustomer";
    internal const string RestaurantEmployeePolicy = "RestaurantEmployee";
    internal const string RestaurantManagerPolicy = "RestaurantManager";

    internal static ScalarOptions ConfigureScalarOptions(this ScalarOptions options, RestaurantConfigurationProvider configurationProvider)
    {
        options.WithTitle(configurationProvider.RestaurantName)
               .WithTheme(ScalarTheme.Kepler)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

        options.AddPreferredSecuritySchemes(RestaurantCustomerPolicy, RestaurantEmployeePolicy, RestaurantManagerPolicy);

        options.Servers = [new("https://localhost:7188")];

        return options;
    }

    internal static IServiceCollection ConfigureAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
                .AddPolicy(RestaurantCustomerPolicy, policy =>
                    policy.AddAuthenticationSchemes(RestaurantCustomerPolicy)
                          .RequireAuthenticatedUser()
                          .RequireRole("customer"))

                .AddPolicy(RestaurantEmployeePolicy, policy =>
                    policy.AddAuthenticationSchemes(RestaurantEmployeePolicy)
                          .RequireAuthenticatedUser()
                          .RequireRole("employee"))

                .AddPolicy(RestaurantManagerPolicy, policy =>
                    policy.AddAuthenticationSchemes(RestaurantManagerPolicy)
                          .RequireAuthenticatedUser()
                          .RequireRole("manager"));

        return services;
    }

    internal static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        string keycloakAuthority = ResolveKeycloakAuthority(configuration);
        string[] validIssuers = ResolveValidIssuers(configuration, keycloakAuthority);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(RestaurantCustomerPolicy, options =>
                {
                    options.Authority = keycloakAuthority;
                    options.Audience = configuration["Authorization:Audience"]!;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuers = validIssuers,
                    };
                })
                .AddJwtBearer(RestaurantEmployeePolicy, options =>
                {
                    options.Authority = keycloakAuthority;
                    options.Audience = configuration["Authorization:Audience"]!;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuers = validIssuers,
                    };
                })
                .AddJwtBearer(RestaurantManagerPolicy, options =>
                {
                    options.Authority = keycloakAuthority;
                    options.Audience = configuration["Authorization:Audience"]!;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuers = validIssuers,
                    };
                })
                .AddPolicyScheme(JwtBearerDefaults.AuthenticationScheme, null, options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        string? scheme = context.Request.Headers["x-scheme"].FirstOrDefault();

                        return scheme switch
                        {
                            RestaurantManagerPolicy => RestaurantManagerPolicy,
                            RestaurantEmployeePolicy => RestaurantEmployeePolicy,
                            RestaurantCustomerPolicy => RestaurantCustomerPolicy,
                            _ => RestaurantCustomerPolicy
                        };
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
