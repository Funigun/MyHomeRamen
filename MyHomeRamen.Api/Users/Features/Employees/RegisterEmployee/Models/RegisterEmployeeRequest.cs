namespace MyHomeRamen.Api.Users.Features.Employees.RegisterEmployee.Models;

public sealed record RegisterEmployeeRequest
(
    string Username,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string TemporaryPassword
);
