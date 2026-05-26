using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Users.Roles.Responses;

namespace MyHomeRamen.Api.Users.Features.Roles.GetAvailableRoles;

public sealed record GetAvailableRolesQuery : IQuery<GetAvailableRolesResponse>;
