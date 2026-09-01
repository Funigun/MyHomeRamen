using MyHomeRamen.Domain.Common.SocialMedia;

namespace MyHomeRamen.Domain.Restaurants.Companies;

internal static class SocialMediaValidator
{
    internal static void Validate(SocialMedia socialMedia)
    {
        if (string.IsNullOrWhiteSpace(socialMedia.Name))
        {
            throw SocialMediaErrors.NameRequired();
        }

        if (socialMedia.Name.Length > SocialMediaConstants.MaxNameLength)
        {
            throw SocialMediaErrors.NameTooLong();
        }

        if (string.IsNullOrWhiteSpace(socialMedia.LogoUrl))
        {
            throw SocialMediaErrors.LogoUrlRequired();
        }

        if (socialMedia.LogoUrl.Length > SocialMediaConstants.MaxLogoUrlLength)
        {
            throw SocialMediaErrors.LogoUrlTooLong();
        }

        if (string.IsNullOrWhiteSpace(socialMedia.Url))
        {
            throw SocialMediaErrors.UrlRequired();
        }

        if (socialMedia.Url.Length > SocialMediaConstants.MaxUrlLength)
        {
            throw SocialMediaErrors.UrlTooLong();
        }
    }
}
