using MyHomeRamen.Common.Contracts.Messaging;

namespace MyHomeRamen.Infrastructure.Messaging.Configuration;

public sealed class QueueConfigurationFactory
{
    public QueueConfiguration CreateQueueConfiguration<T>()
    {
        string queueName = GetQueueNameByMessageType<T>() ?? string.Empty;

        return queueName switch
        {
            MessagesConstants.UserEventsQueue => new UserRegisteredQueueConfig(queueName),
            MessagesConstants.GuestRegisteredQueue => new GuestRegisteredQueueConfig(queueName),
            _ => throw new ArgumentException($"Queue name '{queueName}' is not recognized.", queueName)
        };
    }

    private static string GetQueueNameByMessageType<T>()
    {
        Type messageType = typeof(T);

        return messageType.Name switch
        {
            nameof(UserRegisteredIntegrationEvent) => MessagesConstants.UserEventsQueue,
            nameof(GuestUserCreatedIntegrationEvent) => MessagesConstants.GuestRegisteredQueue,
            _ => throw new ArgumentException($"Message type '{messageType.Name}' is not recognized.")
        };
    }
}
