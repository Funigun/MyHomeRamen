using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;

namespace MyHomeRamen.Features.Identity.Features.Users.UpdateAddress;

public sealed record UpdateAddressCommand(Guid Id, UpdateAddressRequest Request) : ICommand<UpdateAddressResponse>;

