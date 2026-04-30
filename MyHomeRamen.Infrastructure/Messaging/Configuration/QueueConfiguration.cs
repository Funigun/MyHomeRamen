namespace MyHomeRamen.Infrastructure.Messaging.Configuration;

public abstract class QueueConfiguration
{
    public string QueueName { get; init; } = string.Empty;

    public bool Durable { get; init; } = true;

    public bool Exclusive { get; init; }

    public bool AutoDelete { get; init; }

    public IDictionary<string, object?>? Arguments { get; init; }

    protected QueueConfiguration(string queueName)
    {
        if (!MessagesConstants.AvailableQueues.Contains(queueName))
        {
            throw new ArgumentException($"Queue name '{queueName}' is not in the list of available queues.", nameof(queueName));
        }

        QueueName = queueName;
    }
}
