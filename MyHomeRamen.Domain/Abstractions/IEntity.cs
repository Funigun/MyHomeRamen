namespace MyHomeRamen.Domain.Abstractions;

public interface IEntity
{
}

public interface IEntity<out TId> : IEntity
           where TId : IEntityId
{
    TId Id { get; }
}
