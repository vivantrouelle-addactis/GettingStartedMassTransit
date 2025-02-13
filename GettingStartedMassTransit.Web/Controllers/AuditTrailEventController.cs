using GettingStartedMassTransit.Common.EventBus.Abstractions;
using GettingStartedMassTransit.Common.EventBus.Entity.Application;
using GettingStartedMassTransit.Common.EventBus.Entity.AuditTrail;
using GettingStartedMassTransit.Common.EventBus.Entity.Common;
using GettingStartedMassTransit.Common.EventBus.Events;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GettingStartedMassTransit.Publisher.Web.Controllers;
[Route("api/audittrailevent")]
[ApiController]
public class AuditTrailEventController : ControllerBase
{
    private readonly IEventBus _eventBus;
    public AuditTrailEventController(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    [HttpGet("beta")]
    public async Task<Results<Ok, BadRequest>> PublishAuditTrailEventOnBeta()
    {
        AuditTrailEntity<ApplicationBetaEntity> betaEntity = new AuditTrailEntity<ApplicationBetaEntity>
        {
            Id = Guid.NewGuid().ToString(),
            Created = DateTime.UtcNow,
            Description = "AuditTrailEvent applicationBetaEntity",
            Type = "AuditTrailEvent",
            Domain = "Beta",
            User = new UserInfoEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
            },
            Data = new ApplicationBetaEntity
            {
                Id = Guid.NewGuid().ToString(),
                Date = DateTime.UtcNow,
                Description = "Description application beta",
                Study = new Common.EventBus.Entity.Common.StudyEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Study name",
                    Date = DateTime.UtcNow,
                    HourDuration = 1,
                    floatNumber = 30,
                    doubleNumber = 30.0,
                    decimalNumber = 30.0m,
                    IsActive = true
                }
            }
        };

        AuditTrailBetaEvent auditTrailBetaEvent = new AuditTrailBetaEvent
        {
            CreationDate = DateTime.UtcNow,
            AuditTrailBetaEntity = betaEntity
        };
        try
        {
            ApplicationDeltaEntity entity = new ApplicationDeltaEntity
            {
                Description = "AuditTrailEvent applicationBetaEntity",
                Name = "AuditTrailEvent name",
            };

            BsonDocument bsonDocument = entity.ToBsonDocument();

            await _eventBus.PublishBsonAsync(bsonDocument);
        }
        catch(Exception exception)
        {
            Console.WriteLine(exception.ToString());
        }

        return TypedResults.Ok();
    }

    [HttpPost("publish")]
    public async Task<Results<Ok, BadRequest>> PublishAuditTrailEvent([FromBody] AuditTrailEvent auditTrailEvent)
    {
        try
        {
            await _eventBus.SendAuditTrailAsync(auditTrailEvent, "queue:audit-trail");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.ToString());
        }

        return TypedResults.Ok();
    }
}
