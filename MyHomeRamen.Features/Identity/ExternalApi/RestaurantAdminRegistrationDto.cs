namespace MyHomeRamen.Features.Identity.ExternalApi;

public sealed record RestaurantAdminRegistrationDto(
    string UserName,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword,
    Guid CompanyId,
    Guid RestaurantId,
    string IdempotencyKey);
