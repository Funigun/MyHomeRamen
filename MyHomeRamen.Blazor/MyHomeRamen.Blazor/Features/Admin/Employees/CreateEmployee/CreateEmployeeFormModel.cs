namespace MyHomeRamen.Blazor.Features.Admin.Employees.CreateEmployee;

public sealed class CreateEmployeeFormModel
{
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string TemporaryPassword { get; set; } = string.Empty;

    public void Reset()
    {
        Username = string.Empty;
        Email = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        TemporaryPassword = string.Empty;
    }
}
