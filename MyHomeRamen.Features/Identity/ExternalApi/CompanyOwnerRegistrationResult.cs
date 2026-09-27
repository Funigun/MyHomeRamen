namespace MyHomeRamen.Features.Identity.ExternalApi;

public sealed record CompanyOwnerRegistrationResult(Guid? UserId, Guid? CompanyId, string? Error);
