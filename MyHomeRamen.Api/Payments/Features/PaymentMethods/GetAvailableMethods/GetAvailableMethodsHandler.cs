using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Payments.PaymentMethods.Responses;
using MyHomeRamen.Domain.Payments.Database;
using MyHomeRamen.Persistance.Payments.Extensions;

namespace MyHomeRamen.Api.Payments.Features.PaymentMethods.GetAvailableMethods;

public sealed class GetAvailableMethodsHandler(IPaymentsDbContext paymentsDbContext) : IQueryHandler<GetAvailableMethodsQuery, IEnumerable<PaymentMethodResponse>>
{
    public async Task<IEnumerable<PaymentMethodResponse>> Handle(GetAvailableMethodsQuery query, CancellationToken cancellationToken)
    {
        return await paymentsDbContext.PaymentMethods.GetAvailableMethods()
                                                     .Select(method => method.ToResponse())
                                                     .ToListAsync(cancellationToken);
    }
}
