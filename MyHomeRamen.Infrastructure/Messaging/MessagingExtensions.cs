using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Features.Common.Messaging;
using MyHomeRamen.Infrastructure.Messaging.Configuration;

namespace MyHomeRamen.Infrastructure.Messaging;

public static class MessagingExtensions
{
    public static IServiceCollection AddMessagingService(this IServiceCollection services)
    {
        services.AddScoped<IMessagesService, MessagesService>();
        services.AddScoped<QueueConfigurationFactory>();
        return services;
    }
}
