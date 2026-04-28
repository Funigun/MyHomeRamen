namespace MyHomeRamen.Common.Contracts.Messaging;

public record GuestUserCreatedIntegrationEvent(Guid UserId, Guid GuestId);
