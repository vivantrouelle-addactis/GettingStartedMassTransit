using GettingStartedMassTransit.Common.EventBus.Entity.Common;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace GettingStartedMassTransit.Common.EventBus.Entity.Application;

public class ApplicationBetaEntity : Entity<string>
{
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public StudyEntity Study { get; set; }
}
