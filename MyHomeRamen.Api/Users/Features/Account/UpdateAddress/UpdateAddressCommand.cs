using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;

namespace MyHomeRamen.Api.Users.Features.Account.UpdateAddress;

public sealed record UpdateAddressCommand(Guid Id, UpdateAddressRequest Request) : IRequest<UpdateAddressResponse>;
