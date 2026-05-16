using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Users.Features.Account.DeleteAddress;

public record struct DeleteAddressCommand : IRequestId<DeleteAddressCommand>, IRequest<IResult>
{
    public Guid Id { get; set; }
}
