namespace MyHomeRamen.Identity.Api.Features.Admin.CreateEmployee;

public sealed record CreateEmployeeRequest(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string TemporaryPassword);
