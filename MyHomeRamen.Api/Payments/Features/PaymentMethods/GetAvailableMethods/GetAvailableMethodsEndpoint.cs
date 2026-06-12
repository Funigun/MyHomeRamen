using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Payments.PaymentMethods.Responses;

namespace MyHomeRamen.Api.Payments.Features.PaymentMethods.GetAvailableMethods;

public sealed class GetAvailableMethodsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<IEnumerable<PaymentMethodResponse>>("api/payments/available-methods", HandleAsync)
            .WithName("GetAvailableMethodsEndpoint")
            .WithTags("PaymentMethods")
            .AllowAnonymous();
    }

    private static async Task<Results<Ok<IEnumerable<PaymentMethodResponse>>, NotFound>> HandleAsync(
        [FromServices] IQueryHandler<GetAvailableMethodsQuery, IEnumerable<PaymentMethodResponse>> handler,
        CancellationToken cancellationToken)
    {
        GetAvailableMethodsQuery query = new();
        IEnumerable<PaymentMethodResponse> response = await handler.Handle(query, cancellationToken);

        return TypedResults.Ok(response);
    }
}
