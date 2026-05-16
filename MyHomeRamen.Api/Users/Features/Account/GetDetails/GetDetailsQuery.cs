using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;

namespace MyHomeRamen.Api.Users.Features.Account.GetDetails;

public sealed record GetDetailsQuery : IRequest<GetDetailsResponse>;
