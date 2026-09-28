namespace MyHomeRamen.Blazor.Features.Account.Common.Services.Contracts.Users.Account.Responses;

public sealed record GetMeResponse(
    Guid UserId,
    string? FirstName,
    GetMeAdminActionsResponse? AdminActions,
    GetMeOwnerActionsResponse? OwnerActions);

public sealed record GetMeAdminActionsResponse(bool CanViewPanel);

public sealed record GetMeOwnerActionsResponse(bool CanViewPanel);
