using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Decorators;
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
        Type handlerOpenType = typeof(IRequestHandler<>);
        Type handlerWithResponseOpenType = typeof(IRequestHandler<,>);
        Type validatorOpenType = typeof(IValidator<>);
        Type authPolicyOpenType = typeof(IAuthorizationPolicy<>);

        List<Type> noResponseHandlers = assembly.GetExportedTypes()
                                                .Where(t => t.GetInterfaces()
                                                             .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerOpenType))
                                                .ToList();

        foreach (Type type in noResponseHandlers)
        {
            Type interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerOpenType);
            services.AddScoped(interfaceType, type);
        }

        List<Type> withResponseHandlers = assembly.GetExportedTypes()
                                                   .Where(t => t.GetInterfaces()
                                                                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerWithResponseOpenType))
                                                   .ToList();

        foreach (Type type in withResponseHandlers)
        {
            Type interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerWithResponseOpenType);
            services.AddScoped(interfaceType, type);
        }

        services.DecorateHandlersWithPipeline(
            noResponseHandlers,
            handlerOpenType,
            validatorOpenType,
            authPolicyOpenType,
            typeof(ValidationHandlerDecorator<>),
            typeof(AuthorizationHandlerDecorator<>));

        services.DecorateHandlersWithPipeline(
            withResponseHandlers,
            handlerWithResponseOpenType,
            validatorOpenType,
            authPolicyOpenType,
            typeof(ValidationHandlerDecorator<,>),
            typeof(AuthorizationHandlerDecorator<,>));

        return services;
    }

    private static void DecorateHandlersWithPipeline(
        this IServiceCollection services,
        List<Type> handlerImplementations,
        Type handlerOpenType,
        Type validatorOpenType,
        Type authPolicyOpenType,
        Type validationDecoratorOpenType,
        Type authorizationDecoratorOpenType)
    {
        foreach (Type handlerImpl in handlerImplementations)
        {
            Type handlerInterface = handlerImpl.GetInterfaces()
                                               .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerOpenType);

            Type[] requestArgs = handlerInterface.GetGenericArguments();
            Type requestType = requestArgs[0];

            bool hasValidator = assembly.GetExportedTypes()
                                        .Any(t => t.GetInterfaces()
                                                   .Any(i => i.IsGenericType
                                                          && i.GetGenericTypeDefinition() == validatorOpenType
                                                          && i.GetGenericArguments()[0] == requestType));

            bool hasAuthPolicy = assembly.GetExportedTypes()
                                         .Any(t => t.GetInterfaces()
                                                    .Any(i => i.IsGenericType
                                                           && i.GetGenericTypeDefinition() == authPolicyOpenType
                                                           && i.GetGenericArguments()[0] == requestType));

            // Build ordered decorators: lower Order = outermost (first pre-exec, last post-exec).
            // Default orders: Authorization=10, Validation=20, Handler=30.
            // Decorators are applied innermost-first, so apply in reverse order (highest first).
            List<(int order, Type decoratorOpenType)> decorators = [];

            if (hasValidator)
            {
                decorators.Add((20, validationDecoratorOpenType));
            }

            if (hasAuthPolicy)
            {
                decorators.Add((10, authorizationDecoratorOpenType));
            }

            foreach ((int _, Type decoratorOpenType) in decorators.OrderByDescending(d => d.order))
            {
                Type closedDecoratorType = decoratorOpenType.MakeGenericType(requestArgs);
                services.Decorate(handlerInterface, closedDecoratorType);
            }
        }
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
