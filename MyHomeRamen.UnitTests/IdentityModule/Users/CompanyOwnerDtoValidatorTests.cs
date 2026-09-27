using MyHomeRamen.Features.Identity.ExternalApi;
using MyHomeRamen.Features.Identity.Services;

namespace MyHomeRamen.UnitTests.IdentityModule.Users;

public sealed class CompanyOwnerDtoValidatorTests
{
    [Fact]
    public void Validate_ShouldRejectMismatchedPasswords_WhenConfirmPasswordDiffers()
    {
        CompanyOwnerDto companyOwner = new(
            "owner",
            "First",
            "Last",
            "owner@example.com",
            "123456789",
            "password",
            "different",
            Guid.CreateVersion7(),
            "idempotency-key");
        CompanyOwnerDtoValidator validator = new();

        FluentValidation.Results.ValidationResult result = validator.Validate(companyOwner);

        Assert.Contains(result.Errors, error => error.ErrorMessage == "Passwords do not match.");
    }
}
