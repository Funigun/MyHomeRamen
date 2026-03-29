namespace MyHomeRamen.Common.Contracts.Messaging;

public record UserRegisteredIntegrationEvent(
    Guid Id,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    string Role);
