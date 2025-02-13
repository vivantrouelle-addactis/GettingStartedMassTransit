using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace GettingStartedMassTransit.Common.EventBus.Entity.Common;

public class UserInfoEntity
{
    [BsonSerializer(typeof(GuidSerializer))]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string ? LastName { get; set; }
}
