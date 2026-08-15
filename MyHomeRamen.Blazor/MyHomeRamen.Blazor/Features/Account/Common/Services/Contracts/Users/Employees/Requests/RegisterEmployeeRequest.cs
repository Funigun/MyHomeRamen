namespace MyHomeRamen.Blazor.Features.Account.Common.Services.Contracts.Users.Employees.Requests;

public sealed record RegisterEmployeeRequest(
    string Username,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string TemporaryPassword);
