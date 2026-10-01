using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Domain.Restaurants.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;
using MyHomeRamen.Features.Restaurants.Features.Companies.Common;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.SocialMedias.GetSocialMediaForManage;

public sealed record SocialMediaManageDto(Guid Id, string Name, string LogoUrl, string Url);
public sealed record GetSocialMediaForManageResponse(IEnumerable<SocialMediaManageDto> Items);
public sealed record GetSocialMediaForManageQuery : IQuery<GetSocialMediaForManageResponse>;

public sealed class GetSocialMediaForManageAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetSocialMediaForManageQuery>
{
    public Task<bool> Authorize(GetSocialMediaForManageQuery request, CancellationToken cancellationToken)
        => Task.FromResult(currentUser.Permissions.Contains(PermissionConstants.CompanySocialMediaView));
}

public sealed class GetSocialMediaForManageHandler(IRestaurantDbContext dbContext) : IRequestHandler<GetSocialMediaForManageQuery, GetSocialMediaForManageResponse>
{
    public async Task<GetSocialMediaForManageResponse> Handle(GetSocialMediaForManageQuery query, CancellationToken cancellationToken)
    {
        Company company = await dbContext.Company.Load().Single(cancellationToken);
        return new GetSocialMediaForManageResponse(company.Media.Select(media => new SocialMediaManageDto(media.Id, media.Name, media.LogoUrl, media.Url)));
    }
}
