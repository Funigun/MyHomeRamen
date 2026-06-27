namespace MyHomeRamen.Features.Common.Endpoints.Models;

public sealed record PageParameters(int PageNumber = 1, int PageSize = 10);
