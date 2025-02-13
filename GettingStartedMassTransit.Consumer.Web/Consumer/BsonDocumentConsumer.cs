using GettingStartedMassTransit.Common.EventBus.Events;
using GettingStartedMassTransit.Consumer.Web.Models.Settings;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json.Nodes;

namespace GettingStartedMassTransit.Consumer.Web.Consumer;

public class BsonDocumentConsumer : IConsumer<BsonDocument>
{
    private readonly ILogger<BsonDocumentConsumer> _logger;
    private readonly IMongoCollection<BsonDocument> _collection;

    public BsonDocumentConsumer(IMongoClient client, IOptions<AuditTrailDatabaseSettings> settings, ILogger<BsonDocumentConsumer> logger)
    {
        _logger = logger;
        _collection = client.GetDatabase(settings.Value.DatabaseName).GetCollection<BsonDocument>(settings.Value.ApplicationBetaCollectionName);
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BsonDocument> context)
    {
        _logger.LogInformation("BsonDocument consumer: {bson}", context.Message);
        var document = context.Message;
        await _collection.InsertOneAsync(document);
    }
}
