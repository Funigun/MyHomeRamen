namespace MyHomeRamen.Domain.Common.SocialMedia;

public static class SocialMediaErrors
{
    public static DomainException NameRequired() => new("Social media name is required");
    public static DomainException NameTooLong() => new($"Social media name cannot be longer than {SocialMediaConstants.MaxNameLength} characters");
    public static DomainException LogoUrlRequired() => new("Social media logo URL is required");
    public static DomainException LogoUrlTooLong() => new($"Social media logo URL cannot be longer than {SocialMediaConstants.MaxLogoUrlLength} characters");
    public static DomainException UrlRequired() => new("Social media URL is required");
    public static DomainException UrlTooLong() => new($"Social media URL cannot be longer than {SocialMediaConstants.MaxUrlLength} characters");
}
