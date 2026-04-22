using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Identity.Api.Features.Account.Addresses.UpdateAddress.Models;

public record struct UpdateAddressRequestId : IRequestId<UpdateAddressRequestId>
{
    public Guid Id { get; set; }
}
