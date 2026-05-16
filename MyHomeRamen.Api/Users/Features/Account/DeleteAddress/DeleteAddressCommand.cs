using MyHomeRamen.Api.Common.Endpoint.Pipeline;

namespace MyHomeRamen.Api.Users.Features.Account.DeleteAddress;

public record DeleteAddressCommand(Guid Id) : ICommand<IResult>;
