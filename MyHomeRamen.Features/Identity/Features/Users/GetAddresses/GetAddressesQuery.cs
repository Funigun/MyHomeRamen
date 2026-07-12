using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;

namespace MyHomeRamen.Features.Identity.Features.Users.GetAddresses;

public sealed record GetAddressesQuery : IQuery<GetAddressesResponse>;

