using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Users.Features.Account.GetId.Models;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Persistance.Users.Extensions;

namespace MyHomeRamen.Api.Users.Features.Account.GetId;

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
