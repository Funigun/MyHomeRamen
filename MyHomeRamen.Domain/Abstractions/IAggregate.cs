namespace MyHomeRamen.Domain.Abstractions;

public interface IAggregate
{
    IEnumerable<IDomainEvent> Events { get; }
}
