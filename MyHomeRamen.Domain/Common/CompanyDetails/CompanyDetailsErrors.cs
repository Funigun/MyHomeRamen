namespace MyHomeRamen.Domain.Common.CompanyDetails;

public static class CompanyErrors
{
    public static DomainException NameRequired() => new("Company name is required");
    public static DomainException NameTooLong() => new($"Company name cannot be longer than {CompanyConstants.MaxNameLength} characters");
    public static DomainException DescriptionTooLong() => new($"Company description cannot be longer than {CompanyConstants.MaxDescriptionLength} characters");
    public static DomainException LogoUrlTooLong() => new($"Company logo URL cannot be longer than {CompanyConstants.MaxLogoUrlLength} characters");
    public static DomainException BusinessDetailsRequired() => new("Company business details are required");
    public static DomainException LegalNameRequired() => new("Company legal name is required");
    public static DomainException LegalNameTooLong() => new($"Company legal name cannot be longer than {CompanyConstants.MaxLegalNameLength} characters");
    public static DomainException TaxIdRequired() => new("Company tax ID is required");
    public static DomainException TaxIdTooLong() => new($"Company tax ID cannot be longer than {CompanyConstants.MaxTaxIdLength} characters");
}
