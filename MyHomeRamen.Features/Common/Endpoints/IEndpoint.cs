using Microsoft.AspNetCore.Routing;

namespace MyHomeRamen.Features.Common.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder endpointBuilder);
}
