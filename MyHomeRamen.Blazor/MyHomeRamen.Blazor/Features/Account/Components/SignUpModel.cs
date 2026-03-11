using MyHomeRamen.Blazor.Features.Account.SignUp;

namespace MyHomeRamen.Blazor.Features.Account.Components;

public sealed class SignUpModel
{
    public string UserName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string ConfirmPassword { get; set; } = string.Empty;

    public SignUpRequest ToSignUpRequest()
    {
        return new SignUpRequest
        (
            UserName,
            FirstName,
            LastName,
            Email,
            PhoneNumber,
            Password,
            ConfirmPassword
        );
    }
}
