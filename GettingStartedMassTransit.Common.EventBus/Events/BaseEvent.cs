using MongoDB.Bson.Serialization.Attributes;

namespace GettingStartedMassTransit.Common.EventBus.Events;

public record BaseEvent
{
    [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
}
