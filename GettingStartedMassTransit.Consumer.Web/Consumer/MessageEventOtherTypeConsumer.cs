using GettingStartedMassTransit.Common.EventBus.Entity;
using MassTransit;
using MongoDB.Bson;

namespace GettingStartedMassTransit.Consumer.Web.Consumer
{
    public class MessageEventOtherTypeConsumer : IConsumer<Common.EventBus.Events.MessageEventOtherType>
    {
        private readonly ILogger<MessageEventOtherTypeConsumer> _logger;
        public MessageEventOtherTypeConsumer(ILogger<MessageEventOtherTypeConsumer> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<Common.EventBus.Events.MessageEventOtherType> context)
        {
            Common.EventBus.Events.MessageEventOtherType message = context.Message;

            _logger.LogInformation("Message: {Text}", message.Text);

            return Task.CompletedTask;
        }
    }
}
