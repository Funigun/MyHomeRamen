using MyHomeRamen.Blazor.Features.Account.Common.Models;

namespace MyHomeRamen.Blazor.Features.Account.AccountManagement.Components;

public sealed class UserDetailsModel
{
    public string Username { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public static UserDetailsModel FromResponse(GetDetailsResponse response)
    {
        return new UserDetailsModel
        {
            Username = response.Username,
            FirstName = response.FirstName,
            LastName = response.LastName,
            Email = response.Email,
            PhoneNumber = response.PhoneNumber
        };
    }
}
