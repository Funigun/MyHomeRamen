using MyHomeRamen.Blazor.Features.Account.SignUp;

namespace MyHomeRamen.Blazor.Features.Account;

public class CustomerAccountApiClient(HttpClient httpClient)
{
    public async Task CreateAsync(SignUpRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/account/sign-up", request, ct);
        response.EnsureSuccessStatusCode();
    }
}
