using MyHomeRamen.Blazor.Features.Admin.Employees;

namespace MyHomeRamen.Blazor.Features.Account;

public class CustomerAccountApiClient(HttpClient httpClient)
{
    public async Task CreateAsync(CreateEmployeeRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/account/sign-up", request, ct);
        response.EnsureSuccessStatusCode();
    }

    public sealed record CustomerRequest(
        string Username,
        string Email,
        string FirstName,
        string LastName,
        string Password
    );
}
