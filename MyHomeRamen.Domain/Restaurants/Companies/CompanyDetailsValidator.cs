using MyHomeRamen.Domain.Common.CompanyDetails;

namespace MyHomeRamen.Domain.Restaurants.Companies;

internal static class CompanyDetailsValidator
{
    internal static void Validate(CompanyDetails companyDetails)
    {
        if (string.IsNullOrWhiteSpace(companyDetails.Name))
        {
            throw CompanyDetailsErrors.NameRequired();
        }

        if (companyDetails.Name.Length > CompanyDetailsConstants.MaxNameLength)
        {
            throw CompanyDetailsErrors.NameTooLong();
        }

        if (companyDetails.Description?.Length > CompanyDetailsConstants.MaxDescriptionLength)
        {
            throw CompanyDetailsErrors.DescriptionTooLong();
        }

        if (companyDetails.LogoUrl?.Length > CompanyDetailsConstants.MaxLogoUrlLength)
        {
            throw CompanyDetailsErrors.LogoUrlTooLong();
        }
    }
}
