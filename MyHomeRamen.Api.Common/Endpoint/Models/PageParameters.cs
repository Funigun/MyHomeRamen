namespace MyHomeRamen.Api.Common.Endpoint.Models;

public sealed record PageParameters(int PageNumber = 1, int PageSize = 10);
