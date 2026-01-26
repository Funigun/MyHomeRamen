namespace MyHomeRamen.Api.Common.Domain;

public interface IEventProducer
{
    IReadOnlyList<IDomainEvent> Events { get; }
}
