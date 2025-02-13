using GettingStartedMassTransit.Common.EventBus.Events;
using GettingStartedMassTransit.Consumer.Web.Models.Settings;
using GettingStartedMassTransit.Common.EventBus.Entity.Application;
using MassTransit;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Text.Json;
using System.Reflection;

namespace GettingStartedMassTransit.Consumer.Web.Consumer.AuditTrails;

public class AuditTrailEventConsumer : IConsumer<Common.EventBus.Events.AuditTrailEvent>
{
    private readonly IMongoCollection<BsonDocument> _collection;
    private readonly ILogger<AuditTrailEventConsumer> _logger;
    public AuditTrailEventConsumer(IMongoClient client, IOptions<AuditTrailDatabaseSettings> settings, ILogger<AuditTrailEventConsumer> logger)
    {
        _logger = logger;
        _collection = client.GetDatabase(settings.Value.DatabaseName).GetCollection<BsonDocument>(settings.Value.AuditTrailCollectionName);
    }
    public async Task Consume(ConsumeContext<Common.EventBus.Events.AuditTrailEvent> context)
    {
        _logger.LogInformation("AuditTrailEvent: {Message}", context.Message);

        AuditTrailEvent auditTrailEvent = context.Message;

        try
        {
            Assembly assembly = Assembly.Load("GettingStartedMassTransit.Common.EventBus");
            // Deserialize the data based on the EventType type
            Type eventType = assembly.GetType(auditTrailEvent.EventType);

            if (eventType == null)
            {
                _logger.LogError("eventType is null");
                return;
            }

            _logger.LogInformation("AuditTrailEvent: {AuditTrailEvent}", auditTrailEvent);

            BsonDocument bsonAuditTrailEvent = auditTrailEvent.ToBsonDocument(); // Serialize auditTrailEvent to BsonDocument

            _logger.LogInformation("Bson AuditTrailEvent: {Document}", bsonAuditTrailEvent);

            object eventData = JsonSerializer.Deserialize(context.Message.Data.ToString(), eventType);

            BsonDocument bsonData = new BsonDocument();
            if (eventType == typeof(ApplicationDeltaEntity))
            {
                ApplicationDeltaEntity businessEntity = (ApplicationDeltaEntity)eventData;
                _logger.LogInformation("ApplicationBetaEntity: {ApplicationBetaEntity}", businessEntity);

                bsonData = businessEntity.ToBsonDocument();
                _logger.LogInformation("Bson Data: {Document}", bsonData.ToString());
            }
            else if (eventType == typeof(ApplicationTetaEntity))
            {
                ApplicationTetaEntity businessEntity = (ApplicationTetaEntity)eventData;
                _logger.LogInformation("ApplicationTetaEntity: {ApplicationTetaEntity}", businessEntity);

                bsonData = businessEntity.ToBsonDocument();
                _logger.LogInformation("Bson Data: {Document}", bsonData.ToString());
            }
            
            BsonDocument bson = bsonAuditTrailEvent.Merge(new BsonDocument("data", bsonData)); // Merge auditTrailEvent and eventData

            _logger.LogInformation("Bson AuditTrainEvent and Data: {Document}", bson.ToString());

            await _collection.InsertOneAsync(bson);
        }
        catch(Exception exception)
        {
            _logger.LogError(exception, "error while deserialization");
        }
    }
}
