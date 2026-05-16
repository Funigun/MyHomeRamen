using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Users.Features.Account.Register.Models;

public sealed record RegisterRequest
(
    string UserName,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword
) : IRequest;
