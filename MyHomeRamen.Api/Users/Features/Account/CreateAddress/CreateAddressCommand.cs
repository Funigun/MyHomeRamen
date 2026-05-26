using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;

namespace MyHomeRamen.Api.Users.Features.Account.CreateAddress;

public sealed record CreateAddressCommand(CreateAddressRequest Request) : ICommand<CreateAddressResponse>;
