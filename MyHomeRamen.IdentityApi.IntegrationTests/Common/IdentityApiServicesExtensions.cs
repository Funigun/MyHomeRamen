using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Configuration;
using MyHomeRamen.Infrastructure.Cache;
using MyHomeRamen.Persistance.Identity;
using NSubstitute;
using StackExchange.Redis;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common;

internal static class IdentityApiServicesExtensions
{
    private const string CustomerScheme = "RestaurantCustomer";
    private const string EmployeeScheme = "RestaurantEmployee";
    private const string ManagerScheme = "RestaurantManager";

    internal static IServiceCollection ReconfigureIdentityDatabase(this IServiceCollection services, string connectionString)
    {
        ServiceDescriptor? descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<IdentityDbContext>));
        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        });

        return services;
    }

    internal static IServiceCollection ReconfigureIdentityTokenOptions(this IServiceCollection services)
    {
        foreach (string scheme in new[] { CustomerScheme, EmployeeScheme, ManagerScheme })
        {
            services.Configure<JwtBearerOptions>(scheme, options =>
            {
                options.Authority = null;
                options.RequireHttpsMetadata = false;
                options.Configuration = new()
                {
                    Issuer = JwtTokenFactory.Issuer,
                    SigningKeys = { JwtTokenFactory.SecurityKey }
                };
#pragma warning disable CA5404
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = JwtTokenFactory.Issuer,
                    ValidAudience = JwtTokenFactory.Audience,
                    IssuerSigningKey = JwtTokenFactory.SecurityKey,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false
                };
#pragma warning restore CA5404
            });
        }

        return services;
    }

    internal static IServiceCollection ReconfigureCache(this IServiceCollection services)
    {
        services.RemoveAll<RedisCacheOptions>();
        services.RemoveAll<IConnectionMultiplexer>();
        services.RemoveAll<HybridCacheEntryOptions>();
        services.RemoveAll<HybridCache>();

        services.AddCacheService();

        return services;
    }

    internal static IServiceCollection ReplaceWithNoop<T>(this IServiceCollection services)
        where T : class
    {
        services.RemoveAll<T>();
        services.AddSingleton(Substitute.For<T>());
        return services;
    }
}
