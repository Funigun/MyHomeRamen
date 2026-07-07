using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.Features.Identity.Extensions;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.Features.Identity.Features.Users.GetId;

public sealed class GetMyIdHandler(IIdentityDbContext dbContext, ICurrentUser currentUser) : IQueryHandler<GetMyIdQuery, GetMyIdResponse>
{
    public async Task<GetMyIdResponse> Handle(GetMyIdQuery query, CancellationToken cancellationToken)
    {
        Guid? id = await dbContext.User.Query().GetIdByKeycloakId(currentUser.Id, cancellationToken);

        if (id is null)
        {
            throw new InvalidOperationException("Authenticated user not found.");
        }

        return new GetMyIdResponse(id.Value);
    }
}

