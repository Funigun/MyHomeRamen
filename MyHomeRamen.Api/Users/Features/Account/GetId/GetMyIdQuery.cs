using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;

namespace MyHomeRamen.Api.Users.Features.Account.GetId;

public sealed record GetMyIdQuery : IQuery<GetMyIdResponse>;
