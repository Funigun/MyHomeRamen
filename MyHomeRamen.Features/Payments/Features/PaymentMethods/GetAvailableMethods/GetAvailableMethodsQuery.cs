using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Payments.PaymentMethods.Responses;

namespace MyHomeRamen.Features.Payments.Features.PaymentMethods.GetAvailableMethods;

public sealed record GetAvailableMethodsQuery() : IQuery<IEnumerable<GetAvailableMethodsResponse>>;
