using GettingStartedMassTransit.Common.EventBus.Entity.Application;
using GettingStartedMassTransit.Common.EventBus.Entity.AuditTrail;
using System.Text.Json.Serialization;

namespace GettingStartedMassTransit.Common.EventBus.Events;
public record AuditTrailBetaEvent : BaseEvent
{
    public AuditTrailEntity<ApplicationBetaEntity> AuditTrailBetaEntity { get; set; }    
}
