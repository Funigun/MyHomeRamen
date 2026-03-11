namespace MyHomeRamen.Blazor.Features.Account.SignUp;

public sealed record SignUpRequest(
    string UserName,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword
);
