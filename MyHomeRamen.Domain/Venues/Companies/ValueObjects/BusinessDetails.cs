namespace MyHomeRamen.Domain.Venues.Companies.ValueObjects;

public sealed class BusinessDetails
{
    public string LegalName { get; private set; } = string.Empty;
    public string TaxId { get; private set; } = string.Empty;

    private BusinessDetails() { }

    public static BusinessDetails Create(string legalName, string taxId)
    {
        return new BusinessDetails
        {
            LegalName = legalName,
            TaxId = taxId
        };
    }
}
