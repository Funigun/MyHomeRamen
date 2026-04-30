namespace MyHomeRamen.Infrastructure.Messaging.Configuration;

public sealed class UserRegisteredQueueConfig(string queueName) : QueueConfiguration(queueName)
{
}
