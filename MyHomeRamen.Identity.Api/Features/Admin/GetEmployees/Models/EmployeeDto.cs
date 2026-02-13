namespace MyHomeRamen.Identity.Api.Features.Admin.GetEmployees.Models;

public sealed record EmployeeDto
(
    Guid Id,
    string UserName,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber
);
