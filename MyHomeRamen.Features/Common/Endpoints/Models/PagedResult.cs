namespace MyHomeRamen.Features.Common.Endpoints.Models;

public sealed record PagedResult<T>(int TotalItems, IEnumerable<T> Items);
