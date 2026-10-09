using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Identity.Permissions;

namespace MyHomeRamen.Features.Identity.Features.Users.GetId;

public sealed record GetMeQuery : IQuery<GetMeResponse>;

public sealed record AdminNavigationAccess(bool CanSeeAdminPanel, IReadOnlyCollection<string> Sections);

public sealed record GetMeResponse(
    Guid UserId,
    string? FirstName,
    AdminNavigationAccess AdminNavigation);

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

        List<string> sections = [];

        if (currentUser.Permissions.Contains(RestaurantsPermissionConstants.CompanyView))
        {
            sections.Add(AdminSectionConstants.CompanyManagement);
        }

        if (currentUser.Permissions.Contains(RestaurantsPermissionConstants.CompanySocialMediaView))
        {
            sections.Add(AdminSectionConstants.SocialMediaManagement);
        }

        if (currentUser.Permissions.Contains(RestaurantsPermissionConstants.RestaurantsManage))
        {
            sections.Add(AdminSectionConstants.RestaurantsManagement);
        }

        if (currentUser.Permissions.Contains(RestaurantsPermissionConstants.RestaurantsCreate))
        {
            sections.Add(AdminSectionConstants.RestaurantCreation);
        }

        if (currentUser.Permissions.Contains(RestaurantsPermissionConstants.RestaurantManage))
        {
            sections.Add(AdminSectionConstants.RestaurantManagement);
        }

        if (currentUser.Permissions.Contains(MenuPermissionConstants.CanManageProducts))
        {
            sections.Add(AdminSectionConstants.ProductsManagement);
        }

        if (currentUser.Permissions.Contains(MenuPermissionConstants.CanManageIngredients))
        {
            sections.Add(AdminSectionConstants.IngredientsManagement);
        }

        return new GetMeResponse(
            user.Id.Value,
            user.GuestId is null ? user.FirstName : null,
            new AdminNavigationAccess(sections.Count > 0, sections));
    }
}
