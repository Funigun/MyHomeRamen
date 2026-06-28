using MyHomeRamen.Features.Common.Endpoints.Command;
namespace MyHomeRamen.Features.Users.Features.Account.DeleteAddress;

public record DeleteAddressCommand(Guid Id) : ICommand;

