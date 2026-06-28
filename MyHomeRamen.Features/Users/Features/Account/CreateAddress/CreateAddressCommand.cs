using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;

namespace MyHomeRamen.Features.Users.Features.Account.CreateAddress;

public sealed record CreateAddressCommand(CreateAddressRequest Request) : ICommand<CreateAddressResponse>;

