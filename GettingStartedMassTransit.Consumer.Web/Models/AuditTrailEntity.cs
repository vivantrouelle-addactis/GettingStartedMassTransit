using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace GettingStartedMassTransit.Consumer.Web.Models
{
    public class AuditTrailEntity
    {
        public string Id { get; set; }
        public DateTime EventTime { get; set; }
        public string EventSource { get; set; }
        public string EventType { get; set; }
        public string Domain { get; set; }
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
    }
}
