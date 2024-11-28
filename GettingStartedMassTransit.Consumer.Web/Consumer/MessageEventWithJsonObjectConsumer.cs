using GettingStartedMassTransit.Common.EventBus.Entity;
using MassTransit;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace GettingStartedMassTransit.Consumer.Web.Consumer
{
    public class MessageEventWithJsonObjectConsumer : IConsumer<Common.EventBus.Events.MessageEventWithJsonObject>
    {
        private readonly ILogger<MessageEventWithJsonObjectConsumer> _logger;
        public MessageEventWithJsonObjectConsumer(ILogger<MessageEventWithJsonObjectConsumer> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<Common.EventBus.Events.MessageEventWithJsonObject> context)
        {   
            Common.EventBus.Events.MessageEventWithJsonObject message = context.Message;
            
            JsonObject data = message.Data;

            _logger.LogInformation("PropertyA: {eventMessageDataPropertyA}", data["PropertyA"]);
            _logger.LogInformation("PropertyB: {eventMessageDataPropertyB}", data["PropertyB"]);
            _logger.LogInformation("PropertyC: {eventMessageDataPropertyC}", data["PropertyC"]);

            // Serialize the JSON object to a BSON document to store in mongo db
            var bsondocument = BsonDocument.Parse(data.ToString());

            return Task.CompletedTask;
        }
    }
}
