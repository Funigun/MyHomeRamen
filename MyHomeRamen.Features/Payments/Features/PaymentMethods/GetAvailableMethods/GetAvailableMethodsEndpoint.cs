using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Payments.Features.PaymentMethods.GetAvailableMethods;

public sealed record GetAvailableMethodsResponse(Guid Id, string Name, string Image, IEnumerable<AvailableChannelDto> Channels);

public sealed record AvailableChannelDto(Guid Id, string Name, string ImageUrl);

public sealed class GetAvailableMethodsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<IEnumerable<GetAvailableMethodsResponse>>("api/payments/available-methods", HandleAsync)
            .WithName("GetAvailableMethodsEndpoint")
            .WithTags("PaymentMethods")
            .AllowAnonymous();
    }

    private static async Task<Results<Ok<IEnumerable<GetAvailableMethodsResponse>>, NotFound>> HandleAsync(
        [FromServices] IRequestHandler<GetAvailableMethodsQuery, IEnumerable<GetAvailableMethodsResponse>> handler,
        CancellationToken cancellationToken)
    {
        GetAvailableMethodsQuery query = new();
        IEnumerable<GetAvailableMethodsResponse> response = await handler.Handle(query, cancellationToken);

        return TypedResults.Ok(response);
    }
}
