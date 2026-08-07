using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.Features.Identity.Features.Users.GetDetails;

public sealed record GetDetailsQuery : IQuery<GetDetailsResponse>;

public sealed class GetDetailsHandler(IIdentityDbContext dbContext, ICurrentUser currentUser) : IQueryHandler<GetDetailsQuery, GetDetailsResponse>
{
    public async Task<GetDetailsResponse> Handle(GetDetailsQuery query, CancellationToken cancellationToken)
    {
        User? user = await dbContext.User.Query().ById(currentUser.UserId, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("Authenticated user not found.");
        }

        return user.ToGetDetailsResponse();
    }
}

