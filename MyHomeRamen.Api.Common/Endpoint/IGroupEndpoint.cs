using Microsoft.AspNetCore.Routing;

namespace MyHomeRamen.Api.Common.Endpoint;

public interface IGroupEndpoint
{
    string GroupName { get; }

    void Configure(RouteGroupBuilder groupBuilder);
}
