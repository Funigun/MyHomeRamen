using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Identity.Features.Users.GetDetails;

public sealed record GetDetailsQuery : IQuery<GetDetailsResponse>;

public sealed class GetDetailsAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetDetailsQuery>
{
    public async Task<bool> Authorize(GetDetailsQuery request, CancellationToken cancellationToken)
    {
        return currentUser.CanViewUserProfile();
    }
}

public sealed class GetDetailsHandler(IIdentityDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<GetDetailsQuery, GetDetailsResponse>
{
    public async Task<GetDetailsResponse> Handle(GetDetailsQuery query, CancellationToken cancellationToken)
    {
        User? user = await dbContext.User.Query().ById(currentUser.UserId, cancellationToken)
                  ?? throw new InvalidOperationException("Authenticated user not found.");
        
        return user.ToGetDetailsResponse();
    }
}

internal static class Mappings
{
    extension(User user)
    {
        internal GetDetailsResponse ToGetDetailsResponse()
        {
            return new GetDetailsResponse(
                user.UserName!,
                user.FirstName,
                user.LastName,
                user.Email!,
                user.PhoneNumber!);
        }
    }
}
