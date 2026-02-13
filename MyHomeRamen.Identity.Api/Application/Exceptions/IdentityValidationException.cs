using MyHomeRamen.Api.Common.Exceptions;

namespace MyHomeRamen.Identity.Api.Application.Exceptions;

public class IdentityValidationException : ApplicationValidationException
{
    private IdentityValidationException(string message) : base(message)
    {
    }

    public static IdentityValidationException RegistrationFailed(IEnumerable<string> errors)
    {
        IdentityValidationException exception = new("Registration failed. Please check the provided data and try again.")
        {
            Errors = errors,
        };

        return exception;
    }

    public static IdentityValidationException UserNameAlreadyInUse() => new("Registration failed")
    {
        Errors = ["User name already in use"],
    };

    public static IdentityValidationException LogInFailed() => new("Login failed")
    {
        Errors = ["Invalid username or password"],
    };
}
