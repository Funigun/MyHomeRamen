using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Payments.PaymentMethods.Responses;

namespace MyHomeRamen.Api.Payments.Features.PaymentMethods.GetAvailableMethods;

public sealed record GetAvailableMethodsQuery() : IQuery<IEnumerable<GetAvailableMethodsResponse>>;
