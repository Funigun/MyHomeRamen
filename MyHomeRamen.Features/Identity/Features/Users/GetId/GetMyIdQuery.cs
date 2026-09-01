using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Identity.Features.Users.GetId;

public sealed record GetMyIdQuery : IQuery<GetMyIdResponse>;

public sealed class GetMyIdAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetMyIdQuery>
{
    public async Task<bool> Authorize(GetMyIdQuery request, CancellationToken cancellationToken)
    {
        return currentUser.CanViewUserProfile();
    }
}

public sealed class GetMyIdHandler(IIdentityDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<GetMyIdQuery, GetMyIdResponse>
{
    public async Task<GetMyIdResponse> Handle(GetMyIdQuery query, CancellationToken cancellationToken)
    {
        Guid? id = await dbContext.User.Query().GetIdByKeycloakId(currentUser.IdentityId, cancellationToken)
                ?? throw new InvalidOperationException("Authenticated user not found.");

        return new GetMyIdResponse(id.Value);
    }
}
