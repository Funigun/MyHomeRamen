using MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.RegisterOwner.Models;

public sealed class RegisterCompanyOwnerModel
{
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;

    public RegisterCompanyOwnerRequest ToRequest()
    {
        return new RegisterCompanyOwnerRequest(
            UserName,
            FirstName,
            LastName,
            Email,
            PhoneNumber,
            Password,
            ConfirmPassword,
            CompanyName);
    }
}
