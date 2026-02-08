using MyHomeRamen.Api.Common.Endpoint;

namespace MyHomeRamen.Identity.Api.Features.Account;

public class AccountGroup : IGroupEndpoint
{
    public string GroupName { get; } = "Account";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithDescription("Account management group")
                    .WithTags("account");
    }
}
