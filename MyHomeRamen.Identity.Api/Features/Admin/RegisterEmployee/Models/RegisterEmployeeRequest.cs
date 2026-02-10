namespace MyHomeRamen.Identity.Api.Features.Admin.RegisterEmployee.Models;

public sealed record RegisterEmployeeRequest
(
    string UserName,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password
);
