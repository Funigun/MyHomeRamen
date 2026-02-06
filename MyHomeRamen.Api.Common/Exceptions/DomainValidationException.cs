namespace MyHomeRamen.Api.Common.Exceptions;

public abstract class DomainValidationException : Exception
{
    protected DomainValidationException(string message) : base(message)
    {
    }

    protected DomainValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
