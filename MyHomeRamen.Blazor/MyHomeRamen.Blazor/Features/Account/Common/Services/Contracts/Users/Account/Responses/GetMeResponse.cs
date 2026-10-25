namespace MyHomeRamen.Blazor.Features.Account.Common.Services.Contracts.Users.Account.Responses;

public sealed record GetMeResponse(
    Guid UserId,
    string? FirstName,
    GetMeAdminNavigationResponse AdminNavigation);

public sealed record GetMeAdminNavigationResponse(bool CanSeeAdminPanel, IReadOnlyCollection<string> Sections);
