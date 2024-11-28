using GettingStartedMassTransit.Common.EventBus.Abstractions;
using GettingStartedMassTransit.Common.EventBus.Events;
using MassTransit;

namespace GettingStartedMassTransit.Common.EventBus;

public sealed class EventBus : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint;

    public EventBus(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : BaseEvent
    {
        return _publishEndpoint.Publish(message, cancellationToken);
    }
}
