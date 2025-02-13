using MassTransit;

namespace GettingStartedMassTransit.Consumer.Web.Consumer
{
    public class AuditTrailBetaEventConsumer : IConsumer<Common.EventBus.Events.AuditTrailBetaEvent>
    {
        private readonly ILogger<AuditTrailBetaEventConsumer> _logger;
        public AuditTrailBetaEventConsumer(ILogger<AuditTrailBetaEventConsumer> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<Common.EventBus.Events.AuditTrailBetaEvent> context)
        {
            _logger.LogInformation("AuditTrailBetaEvent: {Message}", context.Message);
            return Task.CompletedTask;
        }
    }
}
