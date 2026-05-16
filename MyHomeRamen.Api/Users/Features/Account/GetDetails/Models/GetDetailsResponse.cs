namespace MyHomeRamen.Api.Users.Features.Account.GetDetails.Models;

public sealed record GetDetailsResponse(string Username, string FirstName, string LastName, string Email, string PhoneNumber);
