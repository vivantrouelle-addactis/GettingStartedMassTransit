namespace GettingStartedMassTransit.Common.EventBus.Events;

public record BaseEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
}
