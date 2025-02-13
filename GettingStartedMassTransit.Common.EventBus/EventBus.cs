using GettingStartedMassTransit.Common.EventBus.Abstractions;
using GettingStartedMassTransit.Common.EventBus.Events;
using MassTransit;
using MongoDB.Bson;

namespace GettingStartedMassTransit.Common.EventBus;

public sealed class EventBus : IEventBus
{
    private readonly IBus _bus;

    public EventBus(IBus bus)
    {
        _bus = bus;
    }

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : BaseEvent
    {
        try
        {
            return _bus.Publish(message, cancellationToken);
        }
        catch(Exception ex)
        {
            throw new Exception($"An error occurred while publishing the message: {ex.Message}");
        }
    }

    public Task PublishBsonAsync<T>(T message, CancellationToken cancellationToken = default) where T : BsonDocument
    {
        try
        {
            return _bus.Publish(message, cancellationToken);
        } 
        catch(Exception ex)
        {
            throw new Exception($"An error occurred while publishing the BSON document: {ex.Message}");
        }
    }

    public Task PublishAuditTrailAsync<T>(T message, CancellationToken cancellationToken = default) where T : BaseEvent
    {
        try
        {
            return _bus.Publish(message, cancellationToken);
        }
        catch(Exception ex)
        {
            throw new Exception($"An error occurred while publishing the audit trail event: {ex.Message}");
        }
    }

    public async Task SendAuditTrailAsync<T>(T message, string destinationUrl, CancellationToken cancellation = default)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        if (destinationUrl == null)
        {
            throw new ArgumentNullException(nameof(destinationUrl));
        }

        try
        {
            ISendEndpoint endpoint = await _bus.GetSendEndpoint(new Uri(destinationUrl));
            await endpoint.Send(message);
        }
        catch(Exception exception)
        {
            throw new Exception($"An error occurred while sending the audit trail event: {exception.Message}");
        }
    }
}
