using MyHomeRamen.Api.Common.Endpoint;

namespace MyHomeRamen.Identity.Api.Features.Admin;

public class AdminGroup : IGroupEndpoint
{
    public string GroupName { get; } = "Admin";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithDescription("Admin features")
                    .WithTags("admin")
                    .RequireAuthorization();
    }
}
