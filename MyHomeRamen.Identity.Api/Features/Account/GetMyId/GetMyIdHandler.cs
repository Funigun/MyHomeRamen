using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Identity.Api.Features.Account.GetMyId.Models;
using MyHomeRamen.Persistance.Users.Extensions;

namespace MyHomeRamen.Identity.Api.Features.Account.GetMyId;

public sealed class GetMyIdHandler(IUsersDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<GetMyIdRequest, GetMyIdResponse>
{
    public async Task<GetMyIdResponse> Handle(GetMyIdRequest request, CancellationToken cancellationToken)
    {
        Guid? id = await dbContext.Users.GetIdByKeycloakId(currentUser.Id, cancellationToken);

        if (id is null)
        {
            throw new InvalidOperationException("Authenticated user not found.");
        }

        return new GetMyIdResponse(id.Value);
    }
}
