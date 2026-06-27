using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Persistance.Users.Extensions;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Api.Users.Features.Account.GetId;

public sealed class GetMyIdHandler(IUsersDbContext dbContext, ICurrentUser currentUser) : IQueryHandler<GetMyIdQuery, GetMyIdResponse>
{
    public async Task<GetMyIdResponse> Handle(GetMyIdQuery query, CancellationToken cancellationToken)
    {
        Guid? id = await dbContext.Users.GetIdByKeycloakId(currentUser.Id, cancellationToken);

        if (id is null)
        {
            throw new InvalidOperationException("Authenticated user not found.");
        }

        return new GetMyIdResponse(id.Value);
    }
}
