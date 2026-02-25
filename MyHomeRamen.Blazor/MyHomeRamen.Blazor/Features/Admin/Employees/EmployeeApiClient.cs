namespace MyHomeRamen.Blazor.Features.Admin.Employees;

public sealed class EmployeeApiClient(HttpClient httpClient)
{
    public async Task CreateAsync(CreateEmployeeRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/admin/employee-sign-up", request, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<GetEmployeesResponse> GetEmployeesAsync(CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.GetAsync("/api/admin/employee", ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<GetEmployeesResponse>(ct) ?? default!;
    }

    public async Task<GetRolesResponse> GetAvailableRoles(CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.GetAsync("/api/admin/role", ct);
        response.EnsureSuccessStatusCode();
        string rsp = await response.Content.ReadAsStringAsync();
        return await response.Content.ReadFromJsonAsync<GetRolesResponse>(ct) ?? new([]);
    }
}

public sealed record CreateEmployeeRequest(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string TemporaryPassword);

public sealed record GetEmployeesResponse(IEnumerable<EmployeeDto> Employees);

public sealed record EmployeeDto(
    string Id,
    string Username,
    string Email,
    string FirstName,
    string LastName);

public sealed record GetRolesResponse(IEnumerable<RoleDto> Roles);

public sealed record RoleDto(
    string Id,
    string Name);
