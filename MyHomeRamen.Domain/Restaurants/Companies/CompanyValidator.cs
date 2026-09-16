using MyHomeRamen.Domain.Common.CompanyDetails;

namespace MyHomeRamen.Domain.Restaurants.Companies;

internal static class CompanyValidator
{
    internal static void Validate(Company companyDetails)
    {
        if (string.IsNullOrWhiteSpace(companyDetails.Name) || string.IsNullOrWhiteSpace(companyDetails.NormalizedName))
        {
            throw CompanyErrors.NameRequired();
        }

        if (companyDetails.Name.Length > CompanyConstants.MaxNameLength)
        {
            throw CompanyErrors.NameTooLong();
        }

        if (companyDetails.Description?.Length > CompanyConstants.MaxDescriptionLength)
        {
            throw CompanyErrors.DescriptionTooLong();
        }

        if (companyDetails.LogoUrl?.Length > CompanyConstants.MaxLogoUrlLength)
        {
            throw CompanyErrors.LogoUrlTooLong();
        }
    }
}
