using Microsoft.AspNetCore.Identity;
using MyHomeRamen.Identity.Api.Application.Services;
using MyHomeRamen.Identity.Api.Domain;
using MyHomeRamen.Identity.Api.Persistance;
using Scalar.AspNetCore;

namespace MyHomeRamen.Identity.Api.Presentation;

internal static class DependencyInjection
{
    internal static ScalarOptions ConfigureScalarOptions(this ScalarOptions options)
    {
        options.WithTitle("Recipe Manager Identity API")
               .WithTheme(ScalarTheme.Kepler)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

        options.AddPreferredSecuritySchemes("Bearer");
        options.AddHttpAuthentication("Bearer", o => o.Description = "Provide valid token");

        options.Servers = [new("https://localhost:7188")];

        return options;
    }

    internal static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentityCore<User>()
                .AddRoles<Role>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddApiEndpoints();

        services.AddScoped<AuthorizationService>();

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 10;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredUniqueChars = 3;
            options.User.RequireUniqueEmail = true;
        });

        return services;
    }
}
