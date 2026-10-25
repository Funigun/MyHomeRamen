using MyHomeRamen.Domain.Common.CompanyDetails;

namespace MyHomeRamen.Domain.Restaurants.Companies.ValueObjects;

internal static class BusinessDetailsValidator
{
    internal static void Validate(BusinessDetails businessDetails)
    {
        if (string.IsNullOrWhiteSpace(businessDetails.LegalName))
        {
            throw CompanyErrors.LegalNameRequired();
        }

        if (businessDetails.LegalName.Length > CompanyConstants.MaxLegalNameLength)
        {
            throw CompanyErrors.LegalNameTooLong();
        }

        if (string.IsNullOrWhiteSpace(businessDetails.TaxId))
        {
            throw CompanyErrors.TaxIdRequired();
        }

        if (businessDetails.TaxId.Length > CompanyConstants.MaxTaxIdLength)
        {
            throw CompanyErrors.TaxIdTooLong();
        }
    }
}
