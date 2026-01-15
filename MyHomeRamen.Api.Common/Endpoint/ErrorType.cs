namespace MyHomeRamen.Api.Common.Endpoint;

public enum ErrorType
{
    None,
    NotFound,
    ValidationError,
    Locked,
    Unauthorized,
    Forbidden,
    InternalServerError,
}
