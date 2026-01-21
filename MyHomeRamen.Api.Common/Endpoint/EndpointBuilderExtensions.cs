using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

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

    public static RouteHandlerBuilder MapStandardGet<TResponse>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        RouteHandlerBuilder routeHandler = builder.MapGet(pattern, handler)
                                                  .Produces<TResponse>(StatusCodes.Status200OK)
                                                  .ProducesProblem(StatusCodes.Status404NotFound)
                                                  .ProducesProblem(StatusCodes.Status500InternalServerError)
                                                  .WithMetadata(typeof(TResponse).DeclaringType!);

        return routeHandler;
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

    public static RouteHandlerBuilder MapStandardDelete<TRequest>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        RouteHandlerBuilder routeHandler = builder.MapDelete(pattern, handler)
                                                  .Produces(StatusCodes.Status204NoContent)
                                                  .ProducesProblem(StatusCodes.Status404NotFound)
                                                  .ProducesProblem(StatusCodes.Status500InternalServerError)
                                                  .WithMetadata(typeof(TRequest).DeclaringType!);

        return routeHandler;
    }
}
