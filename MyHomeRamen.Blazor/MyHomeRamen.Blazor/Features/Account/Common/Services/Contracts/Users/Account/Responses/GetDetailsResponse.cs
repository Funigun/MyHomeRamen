namespace MyHomeRamen.Blazor.Features.Account.Common.Services.Contracts.Users.Account.Responses;

public sealed record GetDetailsResponse(string Username, string FirstName, string LastName, string Email, string PhoneNumber);
