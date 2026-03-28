using MyHomeRamen.Api.Common.Authorization;

namespace MyHomeRamen.Api.Menu.Features.GetCategoriesOptions;

public sealed class GetCategoriesOptionsAuthorizationPolicy : IAuthorizationPolicy<GetCategoriesOptionsRequest>
{
    public Task<bool> IsAuthorized(GetCategoriesOptionsRequest request) =>
        Task.FromResult(true);
}
