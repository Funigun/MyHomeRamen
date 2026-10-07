namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

public sealed record GetSocialMediaForManageResponse(IEnumerable<SocialMediaManageResponse> Items);
public sealed record SocialMediaManageResponse(Guid Id, string Name, string LogoUrl, string Url);
public sealed record CreateSocialMediaRequest(string Name, string LogoUrl, string Url);
public sealed record UpdateSocialMediaRequest(string Name, string LogoUrl, string Url);
