using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Api.Common.Filter;

namespace MyHomeRamen.Api.Common.Endpoint;

public static class EndpointBuilderExtensions
{
    public static RouteHandlerBuilder MapStandardPost<TRequest, TResponse>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        RouteHandlerBuilder routeHandler = builder.MapPost(pattern, handler)
                                                  .Produces<TResponse>(StatusCodes.Status201Created)
                                                  .ProducesProblem(StatusCodes.Status400BadRequest)
                                                  .ProducesProblem(StatusCodes.Status404NotFound)
                                                  .ProducesProblem(StatusCodes.Status500InternalServerError)
                                                  .WithMetadata(typeof(TRequest).DeclaringType!);

        return routeHandler;
    }

    public static RouteHandlerBuilder MapStandardValidatedPost<TRequest, TResponse>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        return builder.MapStandardPost<TRequest, TResponse>(pattern, handler)
                      .WithValidationFilter<TRequest>();
    }

    public static RouteHandlerBuilder MapStandardAuthenticatedPost<TRequest, TResponse>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        return builder.MapStandardPost<TRequest, TResponse>(pattern, handler)
                      .WithAuthenticationFilter<TRequest>()
                      .WithValidationFilter<TRequest>();
    }

    public static RouteHandlerBuilder WithValidationFilter<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<ValidationFilter<TRequest>>()
                      .WithMetadata("Uses validation filter");
    }

    public static RouteHandlerBuilder WithAuthenticationFilter<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder.ProducesProblem(StatusCodes.Status403Forbidden)
                      .AddEndpointFilter<AuthorizationFilter<TRequest>>()
                      .WithMetadata("Uses authentication filter");
    }

    public static RouteHandlerBuilder MapStandardGet<TResponse>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        RouteHandlerBuilder routeHandler = builder.MapGet(pattern, handler)
                                                  .Produces<TResponse>(StatusCodes.Status200OK)
                                                  .ProducesProblem(StatusCodes.Status404NotFound)
                                                  .ProducesProblem(StatusCodes.Status500InternalServerError)
                                                  .WithMetadata(typeof(TResponse).DeclaringType!);

        return routeHandler;
    }

    public static RouteHandlerBuilder MapStandardValidatedGet<TRequest, TResponse>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        return builder.MapStandardGet<TResponse>(pattern, handler)
                      .WithValidationFilter<TRequest>()
                      .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    public static RouteHandlerBuilder MapStandardAuthenticatedGet<TRequest, TResponse>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        return builder.MapStandardGet<TResponse>(pattern, handler)
                      .WithAuthenticationFilter<TRequest>();
    }

    public static RouteHandlerBuilder MapStandardPut<TRequest>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        RouteHandlerBuilder routeHandler = builder.MapPut(pattern, handler)
                                                  .Produces(StatusCodes.Status204NoContent)
                                                  .ProducesProblem(StatusCodes.Status400BadRequest)
                                                  .ProducesProblem(StatusCodes.Status404NotFound)
                                                  .ProducesProblem(StatusCodes.Status500InternalServerError)
                                                  .WithMetadata(typeof(TRequest).DeclaringType!);

        return routeHandler;
    }

    public static RouteHandlerBuilder MapStandardValidatedPut<TRequest>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        return builder.MapStandardPut<TRequest>(pattern, handler)
                      .WithValidationFilter<TRequest>();
    }

    public static RouteHandlerBuilder MapStandardValidatedPutWithResponse<TRequest, TResponse>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        RouteHandlerBuilder routeHandler = builder.MapPut(pattern, handler)
                                                  .Produces<TResponse>(StatusCodes.Status200OK)
                                                  .ProducesProblem(StatusCodes.Status400BadRequest)
                                                  .ProducesProblem(StatusCodes.Status404NotFound)
                                                  .ProducesProblem(StatusCodes.Status500InternalServerError)
                                                  .WithMetadata(typeof(TRequest).DeclaringType!);

        return routeHandler.WithValidationFilter<TRequest>();
    }

    public static RouteHandlerBuilder MapStandardAuthenticatedPut<TRequest, TDto>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        return builder.MapStandardPut<TRequest>(pattern, handler)
                      .WithAuthenticationFilter<TRequest>()
                      .WithValidationFilter<TDto>();
    }

    public static RouteHandlerBuilder MapStandardDelete<TRequest>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        RouteHandlerBuilder routeHandler = builder.MapDelete(pattern, handler)
                                                  .Produces(StatusCodes.Status204NoContent)
                                                  .ProducesProblem(StatusCodes.Status404NotFound)
                                                  .ProducesProblem(StatusCodes.Status400BadRequest)
                                                  .ProducesProblem(StatusCodes.Status500InternalServerError)
                                                  .WithMetadata(typeof(TRequest).DeclaringType!);

        return routeHandler;
    }

    public static RouteHandlerBuilder MapStandardValidatedDelete<TRequest>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        return builder.MapStandardDelete<TRequest>(pattern, handler)
                      .WithValidationFilter<TRequest>();
    }

    public static RouteHandlerBuilder MapStandardAuthenticatedDelete<TMarker, TRequest>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        return builder.MapStandardDelete<TMarker>(pattern, handler)
                      .WithAuthenticationFilter<TRequest>();
    }
}
