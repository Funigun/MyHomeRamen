using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;

namespace MyHomeRamen.Api.Users.Features.Account.GetDetails;

public sealed record GetDetailsQuery : IQuery<GetDetailsResponse>;
