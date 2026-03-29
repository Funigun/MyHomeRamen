namespace MyHomeRamen.Api.Common.Domain;

public interface IDomainEventHandler<in TEvent>
           where TEvent : IDomainEvent
{
    Task HandleEvent(TEvent domainEvent, CancellationToken cancellationToken = default);
}
