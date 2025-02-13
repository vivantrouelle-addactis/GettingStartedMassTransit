using GettingStartedMassTransit.Common.EventBus.Entity.Common;
using MassTransit;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Nodes;

namespace GettingStartedMassTransit.Common.EventBus.Events;

public record AuditTrailEvent : BaseEvent
{
    public required DateTime EventTime { get; set; }
    public required string EventSource { get; set; }
    public required string EventType { get; set; }
    public required string Domain { get; set; }
    public UserInfoEntity? User { get; set; }
    [BsonIgnore]
    public string Data { get; set; }
}
