using MyHomeRamen.Features.Payments.Features.Abstractions;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Payments.Features.PaymentMethods.GetAvailableMethods;

public sealed record GetAvailableMethodsQuery() : IQuery<IEnumerable<GetAvailableMethodsResponse>>;

public sealed class GetAvailableMethodsHandler(IPaymentsDbContext paymentsDbContext) : IRequestHandler<GetAvailableMethodsQuery, IEnumerable<GetAvailableMethodsResponse>>
{
    public async Task<IEnumerable<GetAvailableMethodsResponse>> Handle(GetAvailableMethodsQuery query, CancellationToken cancellationToken)
    {
        return await paymentsDbContext.PaymentMethod.Query().GetAvailableMethodsAsync(cancellationToken);
    }
}
