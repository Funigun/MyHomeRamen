namespace MyHomeRamen.Api.Users.Features.Employees.GetEmployees.Models;

public sealed record GetEmployeesResponse(IEnumerable<EmployeeDto> Employees);
