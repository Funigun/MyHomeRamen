using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Features.Common.Messaging;

namespace MyHomeRamen.Infrastructure.Messaging;

public static class MessagingExtensions
{
    public static IServiceCollection AddMessagingService(this IServiceCollection services)
    {
        services.AddScoped<IMessagesService, MessagesService>();
        return services;
    }
}
