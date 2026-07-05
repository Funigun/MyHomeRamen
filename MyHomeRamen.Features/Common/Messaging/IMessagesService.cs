namespace MyHomeRamen.Features.Common.Messaging;

public interface IMessagesService
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken) where T : class;

    Task ConsumeAsync<T>(Func<T, CancellationToken, Task> onMessageReceived, CancellationToken cancellationToken) where T : class;
}
