using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Users.Roles.Responses;

namespace MyHomeRamen.Features.Users.Features.Roles.GetAvailableRoles;

public sealed record GetAvailableRolesQuery : IQuery<GetAvailableRolesResponse>;

