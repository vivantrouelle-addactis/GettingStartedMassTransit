using MongoDB.Bson.Serialization.Attributes;

namespace GettingStartedMassTransit.Common.EventBus.Entity.Common;

public abstract class Entity<T>: BaseEntity, IEntity<T>
{
    [BsonId]
    public T Id { get; set; }
}
