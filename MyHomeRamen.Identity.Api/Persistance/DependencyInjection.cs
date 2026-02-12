using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MyHomeRamen.Identity.Api.Application.Services;
using MyHomeRamen.Identity.Api.Domain;

namespace MyHomeRamen.Identity.Api.Persistance;

internal static class DependencyInjection
{
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

    internal static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer
            (
                configuration[$"RestaurantConfiguration:IdentityConnectionString"]!,
                serverOptions =>
                {
                    serverOptions.CommandTimeout(600);
                    serverOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "identity");
                }
            );
        });

        return services;
    }
}
