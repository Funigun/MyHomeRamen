namespace MyHomeRamen.Blazor.Features.Account.Common.Services.Contracts.Users.Employees.Responses;

public sealed record GetEmployeesResponse(IEnumerable<EmployeeDto> Employees);

public sealed record EmployeeDto(string UserName, string FirstName, string LastName, string Email);
