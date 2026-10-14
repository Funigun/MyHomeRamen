using MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.CompanyDetails.Models;

public sealed record CompanyBusinessDetailsViewModel(string LegalName, string TaxId)
{
    public static CompanyBusinessDetailsViewModel FromResponse(BusinessDetailsDto response)
        => new(response.LegalName, response.TaxId);
}
