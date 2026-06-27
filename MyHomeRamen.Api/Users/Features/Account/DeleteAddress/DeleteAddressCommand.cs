using MyHomeRamen.Features.Common.Endpoints.Command;
namespace MyHomeRamen.Api.Users.Features.Account.DeleteAddress;

public record DeleteAddressCommand(Guid Id) : ICommand;
