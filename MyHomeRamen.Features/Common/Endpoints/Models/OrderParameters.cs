namespace MyHomeRamen.Features.Common.Endpoints.Models;

public sealed record OrderParameters(string SortBy, string SortOrder = "asc");
