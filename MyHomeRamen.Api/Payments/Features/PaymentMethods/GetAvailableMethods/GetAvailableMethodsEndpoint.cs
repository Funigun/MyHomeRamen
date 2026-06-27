using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Payments.PaymentMethods.Responses;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Api.Payments.Features.PaymentMethods.GetAvailableMethods;

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
        [FromServices] IQueryHandler<GetAvailableMethodsQuery, IEnumerable<GetAvailableMethodsResponse>> handler,
        CancellationToken cancellationToken)
    {
        GetAvailableMethodsQuery query = new();
        IEnumerable<GetAvailableMethodsResponse> response = await handler.Handle(query, cancellationToken);

        return TypedResults.Ok(response);
    }
}
