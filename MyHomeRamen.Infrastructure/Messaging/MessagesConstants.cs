namespace MyHomeRamen.Infrastructure.Messaging;

public static class MessagesConstants
{
    public const string UserEventsQueue = "user-events-queue";

    public const string GuestRegisteredQueue = "guest-registered-queue";

    public static readonly HashSet<string> AvailableQueues = [UserEventsQueue, GuestRegisteredQueue];
}
