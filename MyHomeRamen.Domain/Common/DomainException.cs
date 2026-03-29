using MyHomeRamen.Api.Common.Exceptions;

namespace MyHomeRamen.Domain.Common;

public sealed class DomainException : DomainValidationException
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
