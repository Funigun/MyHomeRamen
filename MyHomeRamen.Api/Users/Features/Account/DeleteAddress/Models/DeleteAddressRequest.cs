using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Users.Features.Account.DeleteAddress.Models;

public record struct DeleteAddressRequest : IRequestId<DeleteAddressRequest>, IRequest<IResult>
{
    public Guid Id { get; set; }
}
