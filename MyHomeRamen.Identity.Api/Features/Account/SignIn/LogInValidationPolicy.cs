using FluentValidation;
using MyHomeRamen.Identity.Api.Features.Account.SignIn.Models;

namespace MyHomeRamen.Identity.Api.Features.Account.SignIn;

public class LogInValidationPolicy : AbstractValidator<LogInRequest>
{
    public LogInValidationPolicy()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName is required.")
            .MaximumLength(256).WithMessage("UserName must not exceed 256 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}
