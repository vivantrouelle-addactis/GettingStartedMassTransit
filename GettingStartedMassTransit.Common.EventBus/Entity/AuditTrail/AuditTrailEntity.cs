using Amazon.Runtime.Internal.Settings;
using GettingStartedMassTransit.Common.EventBus.Entity.Common;

namespace GettingStartedMassTransit.Common.EventBus.Entity.AuditTrail;

public class AuditTrailEntity<T> : Entity<string>
{
    public required DateTime Created { get; set; }
    public required string Type { get; set; }
    public required string Description { get; set; }
    public required string Domain { get; set; }
    public UserInfoEntity? User { get; set; }
    public T Data { get; set; }
}
