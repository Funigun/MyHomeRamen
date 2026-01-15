using FluentValidation.Results;

namespace MyHomeRamen.Api.Common.Exceptions;

public class CustomValidationException : ApplicationValidationException
{
    private CustomValidationException(string message) : base(message)
    {
    }

    public static CustomValidationException ValidationFailed(string message, IEnumerable<ValidationFailure> errors)
    {
        CustomValidationException exception = new(message)
        {
            ValidationErrors = errors.GroupBy(error => error.PropertyName)
                                     .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage)),
        };

        return exception;
    }
}
