namespace MyHomeRamen.Domain.Abstractions;

public abstract class Aggregate<TId> : AuditableEntity, IEntity<TId>, IAggregate
                where TId : IEntityId
{
    private readonly List<IDomainEvent> _events = [];

    public TId Id { get; protected set; } = default!;

    public IEnumerable<IDomainEvent> Events => _events.ToList();

    protected void AddEvent(IDomainEvent domainEvent) => _events.Add(domainEvent);
    
    public void ClearEvents() => _events.Clear();
}
