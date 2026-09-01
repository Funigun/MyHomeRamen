namespace MyHomeRamen.Domain.Restaurants.Companies.ValueObjects;

public sealed class BusinessDetails
{
    public string LegalName { get; private set; } = string.Empty;
    public string TaxId { get; private set; } = string.Empty;

    private BusinessDetails() { }

    public static BusinessDetails Create(string legalName, string taxId)
    {
        BusinessDetails businessDetails = new()
        {
            LegalName = legalName,
            TaxId = taxId
        };

        BusinessDetailsValidator.Validate(businessDetails);
        return businessDetails;
    }
}
