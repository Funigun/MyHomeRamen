namespace MyHomeRamen.Features.Identity.ExternalApi;

public sealed record CompanyOwnerDto(
    string UserName,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword,
    Guid CompanyId,
    string IdempotencyKey);
