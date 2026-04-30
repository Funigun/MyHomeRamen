using MyHomeRamen.Api.Common.Messaging;
using MyHomeRamen.Common.Contracts.Messaging;
using MyHomeRamen.Worker.Common;

namespace MyHomeRamen.Worker.MessagesHandler;

public class UserRegistrationHandler(ILogger<UserRegistrationHandler> logger, IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Background messages worker starting at: {time}", DateTimeOffset.Now);

        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IMessagesService messagesService = scope.ServiceProvider.GetRequiredService<IMessagesService>();

        await messagesService.ConsumeAsync<UserRegisteredIntegrationEvent>
        (
            async (integrationEvent, ct) =>
            {
                logger.LogInformation("Processing UserRegisteredIntegrationEvent for User {Id}", integrationEvent.Id);

                IEnumerable<IIntegrationEventHandler<UserRegisteredIntegrationEvent>> handlers = scope.ServiceProvider.GetServices<IIntegrationEventHandler<UserRegisteredIntegrationEvent>>();

                foreach (IIntegrationEventHandler<UserRegisteredIntegrationEvent> handler in handlers)
                {
                    try
                    {
                        logger.LogInformation("Invoking handler {HandlerType}", handler.GetType().Name);
                        await handler.HandleAsync(integrationEvent, ct);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error while invoking handler {HandlerType}", handler.GetType().Name);
                    }
                }
            },
            stoppingToken
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}
