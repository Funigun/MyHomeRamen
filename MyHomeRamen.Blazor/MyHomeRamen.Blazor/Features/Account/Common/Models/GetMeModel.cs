namespace MyHomeRamen.Blazor.Features.Account.Common.Models;

public sealed record GetMeModel(Guid UserId, string? FirstName, GetMeAdminActionsModel? AdminActions, GetMeOwnerActionsModel? OwnerActions);

public sealed record GetMeAdminActionsModel(bool CanViewPanel);

public sealed record GetMeOwnerActionsModel(bool CanViewPanel);
