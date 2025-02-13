using GettingStartedMassTransit.Common.EventBus.Entity.Common;

namespace GettingStartedMassTransit.Common.EventBus.Entity.Application;

public class ApplicationTetaEntity
{
    public string Description { get; set; }
    public TimeOnly Time { get; set; }
    public StudyEntity Study { get; set; }
}
