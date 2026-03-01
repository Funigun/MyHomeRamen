namespace MyHomeRamen.Identity.Api.Features.Admin.RegisterEmployee.Models;

public sealed record RegisterEmployeeRequest
(
    string Username,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string TemporaryPassword
);
