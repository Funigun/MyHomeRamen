using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using MyHomeRamen.Infrastructure.Cache;
using MyHomeRamen.Persistance.Menu;
using MyHomeRamen.Persistance.Orders;
using MyHomeRamen.Persistance.Payments;
using MyHomeRamen.Persistance.Reservations;
using MyHomeRamen.Persistance.ShoppingCart;
using StackExchange.Redis;
using System.Security.Claims;

namespace MyHomeRamen.IntegrationTests.Common.Configuration;

internal static class ApiServicesExtensions
{
    // Mirror the scheme names registered in AuthorizationConfiguration
    private const string CustomerScheme = "RestaurantCustomer";
    private const string EmployeeScheme = "RestaurantEmployee";
    private const string ManagerScheme = "RestaurantManager";

    internal static IServiceCollection ReconfigureTokenOptions(this IServiceCollection services)
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
#pragma warning disable CA5404 // Do not disable token validation checks
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = JwtTokenFactory.Issuer,
                    ValidAudience = JwtTokenFactory.Audience,
                    IssuerSigningKey = JwtTokenFactory.SecurityKey,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false
                };
#pragma warning restore CA5404 // Do not disable token validation checks
            });
        }

        return services;
    }

    internal static IServiceCollection ReconfigureCache(this IServiceCollection services, string connectionString)
    {
        services.RemoveAll<RedisCacheOptions>();
        services.RemoveAll<IConnectionMultiplexer>();
        services.RemoveAll<HybridCacheEntryOptions>();
        services.RemoveAll<HybridCache>();

        IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connectionString);
        services.AddSingleton<IConnectionMultiplexer>(redis);
        services.AddStackExchangeRedisCache(opt => opt.ConnectionMultiplexerFactory = () => Task.FromResult(redis));

        services.AddCacheService();

        return services;
    }

    internal static IServiceCollection ReconfigureClaimsTransformation(this IServiceCollection services)
    {
        services.RemoveAll<IClaimsTransformation>();
        services.AddTransient<IClaimsTransformation, PassThroughClaimsTransformation>();
        return services;
    }

    internal static void ReconfigureDbContext<T>(this IServiceCollection services, string connectionString)
                where T : DbContext
    {
        ServiceDescriptor? descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<T>));

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContext<T>(options =>
        {
            options.UseSqlServer(connectionString);
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        });
    }
}

internal sealed class PassThroughClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal) => Task.FromResult(principal);
}
