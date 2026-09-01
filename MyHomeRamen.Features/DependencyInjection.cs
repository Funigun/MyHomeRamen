using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Identity.ExternalApi;
using MyHomeRamen.Features.Identity.Services;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features;

public static class DependencyInjection
{
    public static IServiceCollection AddPermissionCatalogServices(this IServiceCollection services)
    {
        services.AddScoped<IPermissionCatalogSynchronizer, PermissionCatalogSynchronizer>();

        return services;
    }

    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<CurrentUser>();
        services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<CurrentUser>());

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

    private static bool IsConcreteHandlerImplementation(Type type, Type handlerOpenType)
    {
        return type.IsClass
            && !type.IsAbstract
            && !type.IsInterface
            && !type.IsGenericTypeDefinition
            && type.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == handlerOpenType);
    }

    private static bool HasAuthorizationPolicy(Assembly assembly, Type requestType)
    {
        Type authorizationPolicyType = typeof(IAuthorizationPolicy<>);

        return assembly.GetExportedTypes()
                       .Any(type => type.GetInterfaces()
                                        .Any(@interface => @interface.IsGenericType
                                            && @interface.GetGenericTypeDefinition() == authorizationPolicyType
                                            && @interface.GetGenericArguments()[0] == requestType));
    }

    private static bool IsCommandRequest(Type requestType)
    {
        return typeof(ICommand).IsAssignableFrom(requestType)
            || requestType.GetInterfaces().Any(@interface => @interface.IsGenericType
                && @interface.GetGenericTypeDefinition() == typeof(ICommand<>));
    }

    public static IServiceCollection AddHandlers(this IServiceCollection services, Assembly assembly)
    {
        Type handlerOpenType = typeof(IRequestHandler<,>);
        Type validatorOpenType = typeof(IValidator<>);

        HashSet<Type> validatedRequestTypes = assembly.GetExportedTypes()
                                                      .Where(t => t.GetInterfaces()
                                                                       .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorOpenType))
                                                      .SelectMany(t => t.GetInterfaces()
                                                                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorOpenType)
                                                                        .Select(i => i.GetGenericArguments()[0]))
                                                      .ToHashSet();

        List<Type> handlers = assembly.GetExportedTypes()
                                      .Where(t => IsConcreteHandlerImplementation(t, handlerOpenType))
                                      .ToList();

        foreach (Type type in handlers)
        {
            Type interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerOpenType);
            services.AddScoped(interfaceType, type);

            Type[] handlerArguments = interfaceType.GetGenericArguments();
            Type requestType = handlerArguments[0];
            bool isCommand = IsCommandRequest(requestType);

            if (isCommand || validatedRequestTypes.Contains(requestType))
            {
                services.Decorate(interfaceType, typeof(ValidationHandler<,>).MakeGenericType(handlerArguments));
            }

            if (HasAuthorizationPolicy(assembly, requestType))
            {
                services.Decorate(interfaceType, typeof(AuthorizationHandler<,>).MakeGenericType(handlerArguments));
            }
        }

        return services;
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
