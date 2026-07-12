using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Endpoints.Query;

namespace MyHomeRamen.Features;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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

    private static bool IsConcreteHandlerImplementation(Type type, Type handlerOpenType)
    {
        return type.IsClass
            && !type.IsAbstract
            && !type.IsInterface
            && !type.IsGenericTypeDefinition
            && type.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == handlerOpenType);
    }

    public static IServiceCollection AddCommandHandlers(this IServiceCollection services, Assembly assembly)
    {
        Type noResponseHandlerOpenType = typeof(ICommandHandler<>);
        Type withResponseHandlerOpenType = typeof(ICommandHandler<,>);

        List<Type> noResponseHandlers = assembly.GetExportedTypes()
                                                .Where(t => IsConcreteHandlerImplementation(t, noResponseHandlerOpenType))
                                                .ToList();

        foreach (Type type in noResponseHandlers)
        {
            Type interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == noResponseHandlerOpenType);
            services.AddScoped(interfaceType, type);
            services.Decorate(interfaceType, typeof(CommandValidationHandler<>).MakeGenericType(interfaceType.GetGenericArguments()));
        }

        List<Type> withResponseHandlers = assembly.GetExportedTypes()
                                                   .Where(t => IsConcreteHandlerImplementation(t, withResponseHandlerOpenType))
                                                   .ToList();

        foreach (Type type in withResponseHandlers)
        {
            Type interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == withResponseHandlerOpenType);
            services.AddScoped(interfaceType, type);
            services.Decorate(interfaceType, typeof(CommandValidationHandler<,>).MakeGenericType(interfaceType.GetGenericArguments()));
        }

        return services;
    }

    public static IServiceCollection AddQueryHandlers(this IServiceCollection services, Assembly assembly)
    {
        Type queryHandlerOpenType = typeof(IQueryHandler<,>);
        Type validatorOpenType = typeof(IValidator<>);

        HashSet<Type> validatedQueryTypes = assembly.GetExportedTypes()
                                                    .Where(t => t.GetInterfaces()
                                                                 .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorOpenType))
                                                    .SelectMany(t => t.GetInterfaces()
                                                                      .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorOpenType)
                                                                      .Select(i => i.GetGenericArguments()[0]))
                                                    .ToHashSet();

        List<Type> queryHandlers = assembly.GetExportedTypes()
                                           .Where(t => IsConcreteHandlerImplementation(t, queryHandlerOpenType))
                                           .ToList();

        foreach (Type type in queryHandlers)
        {
            Type interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == queryHandlerOpenType);
            services.AddScoped(interfaceType, type);

            Type queryType = interfaceType.GetGenericArguments()[0];
            if (validatedQueryTypes.Contains(queryType))
            {
                services.Decorate(interfaceType, typeof(QueryValidationHandler<,>).MakeGenericType(interfaceType.GetGenericArguments()));
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
