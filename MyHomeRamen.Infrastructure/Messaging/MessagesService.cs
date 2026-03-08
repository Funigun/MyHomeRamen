using System.Text;
using Microsoft.Extensions.Logging;
using MyHomeRamen.Api.Common.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MyHomeRamen.Infrastructure.Messaging;

public class MessagesService(ILogger<MessagesService> logger, IConnection connection) : IMessagesService
{

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        logger.LogInformation("Publishing message of type {MessageType} to message broker", typeof(T).Name);

        IChannel channel = await connection.CreateChannelAsync(null, cancellationToken);

        await channel.QueueDeclareAsync(
            queue: "user-events-queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "user-events-queue",
            mandatory: true,
            basicProperties: new BasicProperties() { Persistent = true },
            body: Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(message)),
            cancellationToken: cancellationToken);
    }

    public async Task ConsumeAsync<T>(CancellationToken cancellationToken)
    {
        logger.LogInformation("Consuming message of type {MessageType} to message broker", typeof(T).Name);

        IChannel channel = await connection.CreateChannelAsync(null, cancellationToken);

        await channel.QueueDeclareAsync(
            queue: "user-events-queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        AsyncEventingBasicConsumer consumer = new(channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            string message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            T deserializedMessage = System.Text.Json.JsonSerializer.Deserialize<T>(message);
            logger.LogInformation("Received message of type {MessageType} from message broker", typeof(T).Name);

            // Acknowledge the message
            await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken);
        };
    }
}
