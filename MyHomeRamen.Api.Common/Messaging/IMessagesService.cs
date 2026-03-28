namespace MyHomeRamen.Api.Common.Messaging;

public interface IMessagesService
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;

    Task ConsumeAsync<T>(Func<T, CancellationToken, Task> onMessageReceived, CancellationToken cancellationToken = default) where T : class;
}
