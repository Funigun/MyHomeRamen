using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Identity.Permissions;

namespace MyHomeRamen.Features.Identity.Features.Users.GetId;

public sealed record GetMeQuery : IQuery<GetMeResponse>;

public sealed record AdminActions(bool CanViewPanel);

public sealed record OwnerActions(bool CanViewPanel);

public sealed record GetMeResponse(
    Guid UserId,
    string? FirstName,
    AdminActions? AdminActions,
    OwnerActions? OwnerActions);

public sealed class GetMeAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetMeQuery>
{
    public Task<bool> Authorize(GetMeQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(currentUser.UserId != Guid.Empty);
    }
}

public sealed class GetMeHandler(IIdentityDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<GetMeQuery, GetMeResponse>
{
    public async Task<GetMeResponse> Handle(GetMeQuery query, CancellationToken cancellationToken)
    {
        User user = await dbContext.User.Query().ById(currentUser.UserId, cancellationToken)
                    ?? throw new InvalidOperationException("Current user not found.");

        bool canViewAdminPanel = currentUser.Permissions.Contains(RestaurantsPermissionConstants.RestaurantManage);
        bool canViewOwnerPanel = currentUser.Permissions.Contains(RestaurantsPermissionConstants.CompanyView);

        return new GetMeResponse(
            user.Id.Value,
            user.GuestId is null ? user.FirstName : null,
            canViewAdminPanel ? new AdminActions(CanViewPanel: true) : null,
            canViewOwnerPanel ? new OwnerActions(CanViewPanel: true) : null);
    }
}
