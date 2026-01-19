namespace MyHomeRamen.Api.Common.Domain;

public interface IEntity
{
}

public interface IEntity<out TId> : IEntity
           where TId : IEntityId
{
    TId Id { get; }
}
