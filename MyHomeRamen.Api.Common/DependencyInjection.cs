using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Cache;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Hateoas.Builder;
using MyHomeRamen.Api.Common.Hateoas.Common;
using MyHomeRamen.Api.Common.Middleware;

namespace MyHomeRamen.Api.Common;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IHateoasBuilderFactory, HateoasBuilderFactory>();
        services.AddScoped<HateoasLinkService>();
        services.AddScoped<ICurrentUser, CurrentUser>();

        return services;
    }

    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services, Assembly assembly)
    {
        Type authorizationPolicyType = typeof(IAuthorizationPolicy<>);

        List<Type>? types = assembly.GetExportedTypes()
                                    .Where(t => t.GetInterfaces()
                                                     .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == authorizationPolicyType))
                                    .ToList();

        foreach (Type type in types)
        {
            Type interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == authorizationPolicyType);
            services.AddScoped(interfaceType, type);
        }

        return services;
    }

    public static IServiceCollection AddCacheableQueries(this IServiceCollection services, Assembly assembly)
    {
        Type cacheableQueryType = typeof(ICachePolicy<,>);

        List<Type>? types = assembly.GetExportedTypes()
                                    .Where(t => t.GetInterfaces()
                                                     .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == cacheableQueryType))
                                    .ToList();

        foreach (Type type in types)
        {
            Type interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == cacheableQueryType);
            services.AddScoped(interfaceType, type);
        }

        return services;
    }

    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        IEnumerable<Type> endpoints = assembly.GetTypes()
                                              .Where(type => typeof(IEndpoint).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface);

        foreach (Type endpointType in endpoints)
        {
            services.AddTransient(typeof(IEndpoint), endpointType);
        }

        IEnumerable<Type> groups = assembly.GetTypes()
                                           .Where(type => typeof(IGroupEndpoint).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface);

        foreach (Type group in groups)
        {
            services.AddTransient(typeof(IGroupEndpoint), group);
        }

        return services;
    }

    public static IApplicationBuilder UseMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<LoggingMiddleware>();
        app.UseMiddleware<PerformanceMiddleware>();

        return app;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        Dictionary<string, IGroupEndpoint> endpointGroups = app.Services.GetServices<IGroupEndpoint>()
                                                                        .ToDictionary(g => g.GroupName);

        List<IEndpoint> endpoints = app.Services.GetServices<IEndpoint>().ToList();

        Dictionary<string, List<IEndpoint>> groupedEndpoints = [];
        List<IEndpoint> ungroupedEndpoints = [];

        foreach (IEndpoint endpoint in endpoints)
        {
            if (string.IsNullOrEmpty(endpoint.GroupName))
            {
                if (!groupedEndpoints.TryGetValue(endpoint.GroupName, out List<IEndpoint>? groupEndpoints))
                {
                    groupEndpoints = [];
                    groupedEndpoints[endpoint.GroupName] = groupEndpoints;
                }

                groupEndpoints.Add(endpoint);
            }
            else
            {
                ungroupedEndpoints.Add(endpoint);
            }
        }

        foreach (KeyValuePair<string, List<IEndpoint>> group in groupedEndpoints)
        {
            string groupName = group.Key;
            List<IEndpoint> groupEndpoints = group.Value;

#pragma warning disable CA1308 // Normalize strings to uppercase
            string groupRoute = $"api/{groupName.ToLowerInvariant()}";
#pragma warning restore CA1308 // Normalize strings to uppercase
            RouteGroupBuilder routeGroupBuilder = app.MapGroup(groupRoute);

            if (endpointGroups.TryGetValue(groupName, out IGroupEndpoint? endpointGroup))
            {
                endpointGroup.Configure(routeGroupBuilder);
            }

            foreach (IEndpoint endpoint in groupEndpoints)
            {
                endpoint.MapEndpoint(routeGroupBuilder);
            }
        }

        foreach (IEndpoint endpoint in ungroupedEndpoints)
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }

    public static WebApplicationBuilder AddConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                             .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

        return builder;
    }
}
