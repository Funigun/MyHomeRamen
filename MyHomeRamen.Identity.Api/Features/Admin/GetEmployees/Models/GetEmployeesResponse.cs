namespace MyHomeRamen.Identity.Api.Features.Admin.GetEmployees.Models;

public sealed record GetEmployeesResponse(IEnumerable<EmployeeDto> Employees);
