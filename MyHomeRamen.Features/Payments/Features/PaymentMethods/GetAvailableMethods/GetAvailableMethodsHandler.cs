using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Payments.PaymentMethods.Responses;
using MyHomeRamen.Features.Payments.Features.Abstractions;

namespace MyHomeRamen.Features.Payments.Features.PaymentMethods.GetAvailableMethods;

public sealed class GetAvailableMethodsHandler(IPaymentsDbContext paymentsDbContext) : IQueryHandler<GetAvailableMethodsQuery, IEnumerable<GetAvailableMethodsResponse>>
{
    public async Task<IEnumerable<GetAvailableMethodsResponse>> Handle(GetAvailableMethodsQuery query, CancellationToken cancellationToken)
    {
        return await paymentsDbContext.PaymentMethod.Query().GetAvailableMethodsAsync(cancellationToken);
    }
}
