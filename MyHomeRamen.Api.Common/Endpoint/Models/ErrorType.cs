namespace MyHomeRamen.Api.Common.Endpoint.Models;

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
