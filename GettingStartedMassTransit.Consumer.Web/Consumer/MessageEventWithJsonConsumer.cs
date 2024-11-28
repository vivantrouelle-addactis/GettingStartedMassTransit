using GettingStartedMassTransit.Common.EventBus.Entity;
using MassTransit;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GettingStartedMassTransit.Consumer.Web.Consumer
{
    public class MessageEventWithJsonConsumer : IConsumer<Common.EventBus.Events.MessageEventWithJson>
    {
        private readonly ILogger<MessageEventWithJsonConsumer> _logger;
        public MessageEventWithJsonConsumer(ILogger<MessageEventWithJsonConsumer> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<Common.EventBus.Events.MessageEventWithJson> context)
        {
            Common.EventBus.Events.MessageEventWithJson message = context.Message;
            
            ApplicationEntity? entity = JsonConvert.DeserializeObject<ApplicationEntity>(message.Data);
            
            _logger.LogInformation("PropertyA: {eventMessageDataPropertyA}", entity.PropertyA);
            _logger.LogInformation("PropertyB: {eventMessageDataPropertyB}", entity.PropertyB);
            _logger.LogInformation("PropertyC: {eventMessageDataPropertyC}", entity.PropertyC);

            // Serialize the JSON object to a BSON document to store in mongo db
            var bsondocument = BsonDocument.Parse(message.Data);

            return Task.CompletedTask;
        }
    }
}
