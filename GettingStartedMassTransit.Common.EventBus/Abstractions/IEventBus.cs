using GettingStartedMassTransit.Common.EventBus.Events;

namespace GettingStartedMassTransit.Common.EventBus.Abstractions;

public interface IEventBus
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : BaseEvent;
}
