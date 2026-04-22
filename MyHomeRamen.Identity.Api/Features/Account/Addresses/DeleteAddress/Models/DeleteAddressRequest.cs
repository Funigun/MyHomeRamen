using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Identity.Api.Features.Account.Addresses.DeleteAddress.Models;

public record struct DeleteAddressRequest : IRequestId<DeleteAddressRequest>, IRequest<IResult>
{
    public Guid Id { get; set; }
}
