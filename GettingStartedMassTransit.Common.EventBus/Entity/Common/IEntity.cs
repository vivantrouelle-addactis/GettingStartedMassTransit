namespace GettingStartedMassTransit.Common.EventBus.Entity.Common;

public interface IEntity<T>
{
    T Id { get; set; }
}
