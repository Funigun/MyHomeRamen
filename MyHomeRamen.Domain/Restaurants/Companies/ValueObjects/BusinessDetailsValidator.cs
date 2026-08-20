using MyHomeRamen.Domain.Common.CompanyDetails;

namespace MyHomeRamen.Domain.Restaurants.Companies.ValueObjects;

internal static class BusinessDetailsValidator
{
    internal static void Validate(BusinessDetails businessDetails)
    {
        if (string.IsNullOrWhiteSpace(businessDetails.LegalName))
        {
            throw CompanyDetailsErrors.LegalNameRequired();
        }

        if (businessDetails.LegalName.Length > CompanyDetailsConstants.MaxLegalNameLength)
        {
            throw CompanyDetailsErrors.LegalNameTooLong();
        }

        if (string.IsNullOrWhiteSpace(businessDetails.TaxId))
        {
            throw CompanyDetailsErrors.TaxIdRequired();
        }

        if (businessDetails.TaxId.Length > CompanyDetailsConstants.MaxTaxIdLength)
        {
            throw CompanyDetailsErrors.TaxIdTooLong();
        }
    }
}
