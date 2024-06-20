using MassTransit;

namespace GettingStartedMassTransit.Web
{
    public class MessageConsumer : IConsumer<Message>
    {
        private readonly ILogger<MessageConsumer> _logger;

        public MessageConsumer(ILogger<MessageConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<Message> context)
        {
            _logger.LogInformation("Received message: {message}", context.Message.Text);
            return Task.CompletedTask;
        }
    }
}