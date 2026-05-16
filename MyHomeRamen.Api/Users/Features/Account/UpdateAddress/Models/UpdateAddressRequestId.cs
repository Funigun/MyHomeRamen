using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Users.Features.Account.UpdateAddress.Models;

public record struct UpdateAddressRequestId : IRequestId<UpdateAddressRequestId>
{
    public Guid Id { get; set; }
}
