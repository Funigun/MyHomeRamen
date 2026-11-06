namespace MyHomeRamen.Blazor.Features.Account.Common.Models;

public sealed record GetMeModel(Guid UserId, string? FirstName, string? Destination, GetMeAdminNavigationModel AdminNavigation);

public sealed record GetMeAdminNavigationModel(bool CanSeeAdminPanel, IReadOnlyCollection<string> Sections);
