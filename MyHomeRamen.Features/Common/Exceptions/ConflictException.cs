namespace MyHomeRamen.Features.Common.Exceptions;

public sealed class ConflictException(string message) : Exception(message);
