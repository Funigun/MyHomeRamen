namespace MyHomeRamen.Domain.Abstractions;

public interface IEventProducer
{
    IReadOnlyList<IDomainEvent> Events { get; }
}
