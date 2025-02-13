using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace GettingStartedMassTransit.Common.EventBus.Entity.Common;

public class StudyEntity
{
    [BsonSerializer(typeof(GuidSerializer))]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public int HourDuration { get; set; }
    public float floatNumber { get; set; }
    public double doubleNumber { get; set; }
    public decimal decimalNumber { get; set; }
    public bool IsActive { get; set; }
}
