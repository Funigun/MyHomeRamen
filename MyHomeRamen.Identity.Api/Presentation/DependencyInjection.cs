using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Domain.Users.Database;
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

    internal static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = configuration["Authorization:Issuer"],
                        ValidAudience = configuration["Authorization:Audience"],
                    };
                })
                .AddJwtBearer(KeycloakBearerScheme, options =>
                {
                    // Keycloak OIDC discovery — tokens are validated against Keycloak's public keys
                    options.Authority = configuration["KeycloakAdmin:Authority"];
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        NameClaimType = "preferred_username",
                        RoleClaimType = ClaimTypes.Role,
                    };
                });

        services.AddTransient<IClaimsTransformation, KeycloakRolesClaimsTransformation>();

        return services;
    }

    internal static IServiceCollection ConfigureAuthorizationPolicies(this IServiceCollection services, string policy)
    {
        services.AddAuthorizationBuilder()
                .AddPolicy(policy, policy => policy.RequireAuthenticatedUser())
                .AddPolicy(AdminPolicy, policy =>
                    policy.AddAuthenticationSchemes(KeycloakBearerScheme)
                          .RequireAuthenticatedUser()
                          .RequireRole("admin"));

        return services;
    }

    internal static async Task InitDatabase(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        using IUsersDbContext dbContext = scope.ServiceProvider.GetRequiredService<IUsersDbContext>();

        await dbContext.Migrate(CancellationToken.None);
    }
}
