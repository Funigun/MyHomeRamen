using Microsoft.AspNetCore.Routing;

namespace MyHomeRamen.Api.Common.Endpoint;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder endpointBuilder);
}
