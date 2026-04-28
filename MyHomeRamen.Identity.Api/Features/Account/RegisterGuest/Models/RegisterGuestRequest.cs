using System;
using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Identity.Api.Features.Account.RegisterGuest.Models;

public record RegisterGuestRequest(Guid? ExistingGuestId) : IRequest<RegisterGuestResponse>;
