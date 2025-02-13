using GettingStartedMassTransit.Common.EventBus.Events;
using MongoDB.Bson.Serialization.Attributes;

namespace GettingStartedMassTransit.Common.EventBus.Abstractions;

public interface IEventBus
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : BaseEvent;
    Task PublishBsonAsync<T>(T message, CancellationToken cancellationToken = default) where T : MongoDB.Bson.BsonDocument;
    Task PublishAuditTrailAsync<T>(T message, CancellationToken cancellationToken = default) where T : BaseEvent;
    Task SendAuditTrailAsync<T>(T message, string destinationUrl, CancellationToken cancellation = default);


}
