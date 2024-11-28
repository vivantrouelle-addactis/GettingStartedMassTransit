using System.Text.Json.Nodes;

namespace GettingStartedMassTransit.Common.EventBus.Events
{
    public record MessageEventWithJsonObject : BaseEvent
    {
        public DateTime EventTime { get; set; }
        public string EventSource { get; set; }
        public string EventType { get; set; }
        public string Domain { get; set; }
        public Guid UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public JsonObject Data { get; set; }
    }
}
