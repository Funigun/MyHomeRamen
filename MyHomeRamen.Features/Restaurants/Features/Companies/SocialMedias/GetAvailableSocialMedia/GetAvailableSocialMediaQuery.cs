using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;
using MyHomeRamen.Features.Restaurants.Features.Companies.Common;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.SocialMedias.GetAvailableSocialMedia;

public sealed record SocialMediaAvailableDto(string Name, string LogoUrl, string Url);
public sealed record GetAvailableSocialMediaResponse(IEnumerable<SocialMediaAvailableDto> Items);
public sealed record GetAvailableSocialMediaQuery : IQuery<GetAvailableSocialMediaResponse>;

public sealed class GetAvailableSocialMediaHandler(IRestaurantDbContext dbContext) : IRequestHandler<GetAvailableSocialMediaQuery, GetAvailableSocialMediaResponse>
{
    public async Task<GetAvailableSocialMediaResponse> Handle(GetAvailableSocialMediaQuery query, CancellationToken cancellationToken)
    {
        Company company = await dbContext.Company.Load().Single(cancellationToken);
        return new GetAvailableSocialMediaResponse(company.Media.Select(media => new SocialMediaAvailableDto(media.Name, media.LogoUrl, media.Url)));
    }
}
