using MyHomeRamen.Api.Common.Messaging;
using MyHomeRamen.Common.Contracts.Messaging;
using MyHomeRamen.Worker.Common;

namespace MyHomeRamen.Worker.MessagesHandler;

internal class GuestRegistrationHandler(ILogger<GuestRegistrationHandler> logger, IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Background messages worker starting at: {time}", DateTimeOffset.Now);

        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IMessagesService messagesService = scope.ServiceProvider.GetRequiredService<IMessagesService>();

        await messagesService.ConsumeAsync<GuestUserCreatedIntegrationEvent>
        (
            async (integrationEvent, ct) =>
            {
                logger.LogInformation("Processing GuestRegisteredIntegrationEvent for Guest {Id}", integrationEvent.GuestId);

                IEnumerable<IIntegrationEventHandler<GuestUserCreatedIntegrationEvent>> handlers = scope.ServiceProvider.GetServices<IIntegrationEventHandler<GuestUserCreatedIntegrationEvent>>();

                foreach (IIntegrationEventHandler<GuestUserCreatedIntegrationEvent> handler in handlers)
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
