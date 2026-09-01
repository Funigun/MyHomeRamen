namespace MyHomeRamen.Infrastructure.Messaging.Configuration;

public static class QueueConfigurationFactory
{
    public static QueueConfiguration CreateQueueConfiguration<T>()
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
            _ => throw new ArgumentException($"Message type '{messageType.Name}' is not recognized.")
        };
    }
}
