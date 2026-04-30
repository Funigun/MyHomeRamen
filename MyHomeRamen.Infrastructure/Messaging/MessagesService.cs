using System.Text;
using Microsoft.Extensions.Logging;
using MyHomeRamen.Api.Common.Messaging;
using MyHomeRamen.Infrastructure.Messaging.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MyHomeRamen.Infrastructure.Messaging;

public class MessagesService(ILogger<MessagesService> logger, IConnection connection, QueueConfigurationFactory queueConfigurationFactory) : IMessagesService
{
    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
           where T : class
    {
        logger.LogInformation("Publishing message of type {MessageType} to message broker", typeof(T).Name);

        IChannel channel = await connection.CreateChannelAsync(null, cancellationToken);

        QueueConfiguration config = queueConfigurationFactory.CreateQueueConfiguration<T>();

        await channel.QueueDeclareAsync(
            queue: config.QueueName,
            durable: config.Durable,
            exclusive: config.Exclusive,
            autoDelete: config.AutoDelete,
            arguments: config.Arguments,
            cancellationToken: cancellationToken);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: config.QueueName,
            mandatory: true,
            basicProperties: new BasicProperties() { Persistent = true },
            body: Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(message)),
            cancellationToken: cancellationToken);
    }

    public async Task ConsumeAsync<T>(Func<T, CancellationToken, Task> onMessageReceived, CancellationToken cancellationToken = default) 
           where T : class
    {
        logger.LogInformation("Consuming message of type {MessageType} from message broker", typeof(T).Name);

        QueueConfiguration config = queueConfigurationFactory.CreateQueueConfiguration<T>();

        IChannel channel = await connection.CreateChannelAsync(null, cancellationToken);

        await channel.QueueDeclareAsync(
            queue: config.QueueName,
            durable: config.Durable,
            exclusive: config.Exclusive,
            autoDelete: config.AutoDelete,
            arguments: config.Arguments,
            cancellationToken: cancellationToken);

        AsyncEventingBasicConsumer consumer = new(channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            try
            {
                string message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                T deserializedMessage = System.Text.Json.JsonSerializer.Deserialize<T>(message)!;

                logger.LogInformation("Received message of type {MessageType} from message broker", typeof(T).Name);

                await onMessageReceived(deserializedMessage, cancellationToken);

                // Acknowledge the message
                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing message of type {MessageType}", typeof(T).Name);
            }
        };

        await channel.BasicConsumeAsync(
            queue: config.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);
    }
}
