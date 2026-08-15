namespace MyHomeRamen.Blazor.Features.Account.Common.Services.Contracts.Users.Account.Requests;

public sealed record RegisterRequest(
    string UserName,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword);
