using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Users.Roles.Responses;

namespace MyHomeRamen.Api.Users.Features.Roles.GetAvailableRoles;

public sealed record GetAvailableRolesQuery : IRequest<GetAvailableRolesResponse>;
