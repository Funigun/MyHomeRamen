namespace MyHomeRamen.Common.Contracts.Users.Account.Requests;

public sealed record RegisterGuestRequest(Guid? ExistingGuestId);
