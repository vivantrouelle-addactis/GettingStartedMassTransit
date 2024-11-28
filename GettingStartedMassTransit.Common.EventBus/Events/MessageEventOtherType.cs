namespace GettingStartedMassTransit.Common.EventBus.Events
{
    public record MessageEventOtherType : BaseEvent
    {
        public string Text { get; set; }
    }
}
