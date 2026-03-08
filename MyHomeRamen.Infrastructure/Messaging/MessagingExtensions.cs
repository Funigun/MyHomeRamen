using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyHomeRamen.Api.Common.Messaging;

namespace MyHomeRamen.Infrastructure.Messaging;

public static class MessagingExtensions
{
    public static IServiceCollection AddMessagingService(this IServiceCollection services)
    {
        services.TryAddSingleton<IMessagesService, MessagesService>();
        return services;
    }
}
