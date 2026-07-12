using MyHomeRamen.Features.Common.Endpoints.Command;
namespace MyHomeRamen.Features.Identity.Features.Users.DeleteAddress;

public record DeleteAddressCommand(Guid Id) : ICommand;

