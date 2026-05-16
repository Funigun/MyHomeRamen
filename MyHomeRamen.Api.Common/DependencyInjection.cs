using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Api.Common.Middleware;

namespace MyHomeRamen.Api.Common;

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

    public static IServiceCollection AddCommandHandlers(this IServiceCollection services, Assembly assembly)
    {
        Type noResponseHandlerOpenType = typeof(ICommandHandler<>);
        Type withResponseHandlerOpenType = typeof(ICommandHandler<,>);

        List<Type> noResponseHandlers = assembly.GetExportedTypes()
                                                .Where(t => t.GetInterfaces()
                                                             .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == noResponseHandlerOpenType))
                                                .ToList();

        foreach (Type type in noResponseHandlers)
        {
            Type interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == noResponseHandlerOpenType);
            services.AddScoped(interfaceType, type);
            services.Decorate(interfaceType, typeof(CommandValidationHandler<>).MakeGenericType(interfaceType.GetGenericArguments()));
        }

        List<Type> withResponseHandlers = assembly.GetExportedTypes()
                                                   .Where(t => t.GetInterfaces()
                                                                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == withResponseHandlerOpenType))
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

        List<Type> queryHandlers = assembly.GetExportedTypes()
                                           .Where(t => t.GetInterfaces()
                                                        .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == queryHandlerOpenType))
                                           .ToList();

        foreach (Type type in queryHandlers)
        {
            Type interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == queryHandlerOpenType);
            services.AddScoped(interfaceType, type);
            services.Decorate(interfaceType, typeof(QueryValidationHandler<,>).MakeGenericType(interfaceType.GetGenericArguments()));
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
