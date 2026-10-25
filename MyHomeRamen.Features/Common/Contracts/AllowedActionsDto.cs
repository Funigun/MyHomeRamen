namespace MyHomeRamen.Features.Common.Contracts;

public sealed record AllowedActionsDto(bool CanCreate, bool CanEdit, bool CanDelete);
