using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyHomeRamen.Identity.Api.Application;
using MyHomeRamen.Identity.Api.Application.Services;
using MyHomeRamen.Identity.Api.Domain;
using MyHomeRamen.Identity.Api.Persistance;
using Scalar.AspNetCore;

namespace MyHomeRamen.Identity.Api.Presentation;

internal static class DependencyInjection
{
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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JwtSettings:Key"]!)),
                        ValidIssuer = configuration["JwtSettings:Issuer"],
                        ValidAudience = configuration["JwtSettings:Audience"],
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                    };
                });

        return services;
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

    internal static IServiceCollection ConfigureDatabase(this IServiceCollection services, RestaurantConfigurationProvider configurationProvider)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql
            (
                configurationProvider.ConnectionString,
                config => config.CommandTimeout(300)
            );
        });

        services.AddScoped<DbService>();

        return services;
    }

    public static async Task InitDatabase(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        DbService dbService = scope.ServiceProvider.GetRequiredService<DbService>();
        await dbService.SeedDatabase();
    }
}
