namespace MyHomeRamen.Infrastructure.Messaging.Configuration;

public sealed class GuestRegisteredQueueConfig(string queueName) : QueueConfiguration(queueName)
{
}
