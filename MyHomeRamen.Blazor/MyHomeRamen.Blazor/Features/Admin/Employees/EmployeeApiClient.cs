using System.Net.Http.Json;

namespace MyHomeRamen.Blazor.Features.Admin.Employees;

public sealed class EmployeeApiClient(HttpClient httpClient)
{
    public async Task CreateAsync(CreateEmployeeRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/admin/employees", request, ct);
        response.EnsureSuccessStatusCode();
    }
}

public sealed record CreateEmployeeRequest(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string TemporaryPassword);
