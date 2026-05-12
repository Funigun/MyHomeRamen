using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
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

    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        IEnumerable<Type> endpoints = assembly.GetTypes()
                                              .Where(type => typeof(IEndpoint).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface);

        foreach (Type endpointType in endpoints)
        {
            services.AddTransient(typeof(IEndpoint), endpointType);
        }

        return services;
    }

    public static IServiceCollection AddEndpointHandlers(this IServiceCollection services, Assembly assembly)
    {
        Type handler = typeof(IRequestHandler<>);
        Type handlerType = typeof(IRequestHandler<,>);

        List<Type>? types = assembly.GetExportedTypes()
                                    .Where(t => t.GetInterfaces()
                                                     .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handler))
                                    .ToList();

        foreach (Type type in types)
        {
            Type interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == handler);
            services.AddScoped(interfaceType, type);
        }

        types = assembly.GetExportedTypes()
                        .Where(t => t.GetInterfaces()
                                         .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType))
                        .ToList();

        foreach (Type type in types)
        {
            Type interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType);
            services.AddScoped(interfaceType, type);
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
        IEnumerable<IEndpoint> endpoints = app.Services.GetServices<IEndpoint>();

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}
