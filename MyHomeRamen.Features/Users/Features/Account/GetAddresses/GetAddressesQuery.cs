using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;

namespace MyHomeRamen.Features.Users.Features.Account.GetAddresses;

public sealed record GetAddressesQuery : IQuery<GetAddressesResponse>;

