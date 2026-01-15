using Microsoft.AspNetCore.Routing;

namespace MyHomeRamen.Api.Common.Endpoint;

public interface IEndpoint
{
    string GroupName { get; init; }

    void MapEndpoint(IEndpointRouteBuilder endpoints);
}
