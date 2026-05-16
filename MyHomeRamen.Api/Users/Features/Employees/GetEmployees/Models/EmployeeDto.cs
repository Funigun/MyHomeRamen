namespace MyHomeRamen.Api.Users.Features.Employees.GetEmployees.Models;

public sealed record EmployeeDto
(
    Guid Id,
    string UserName,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber
);
