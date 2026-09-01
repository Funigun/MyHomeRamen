using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Configurations;
using Scalar.AspNetCore;

namespace MyHomeRamen.Api.Authorization;

internal static class AuthorizationDependencyInjection
{
    internal static ScalarOptions ConfigureScalarOptions(this ScalarOptions options, RestaurantConfigurationProvider configurationProvider)
    {
        options.WithTitle(configurationProvider.RestaurantName)
               .WithTheme(ScalarTheme.Kepler)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

        options.AddPreferredSecuritySchemes("Bearer");

        options.Servers = [new("https://localhost:7460")];

        return options;
    }

    internal static IServiceCollection ConfigureAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
                .AddPolicy(AuthorizationPolicies.AuthenticatedUserPolicy, policy =>
                    policy.AddAuthenticationSchemes(AuthorizationPolicies.AuthenticatedUserPolicy)
                          .RequireAuthenticatedUser());

        return services;
    }

    internal static IServiceCollection ConfigureAuthentication(this IServiceCollection services, AuthorizationConfiguration authConfig)
    {
        string[] validIssuers = [authConfig.Issuer];

        services.AddAuthentication(AuthorizationPolicies.AuthenticatedUserPolicy)
                .AddJwtBearer(AuthorizationPolicies.AuthenticatedUserPolicy, options =>
                {
                    options.Authority = authConfig.Authority;
                    options.Audience = authConfig.Audience;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuers = validIssuers,
                    };
                });

        services.AddTransient<IClaimsTransformation, KeycloakClaimsTransformation>();

        return services;
    }
}
