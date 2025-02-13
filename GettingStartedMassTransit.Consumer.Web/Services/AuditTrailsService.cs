using GettingStartedMassTransit.Common.EventBus.Entity.Application;
using GettingStartedMassTransit.Common.EventBus.Entity.AuditTrail;
using GettingStartedMassTransit.Consumer.Web.Models.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GettingStartedMassTransit.Consumer.Web.Services;

public class AuditTrailsService<T>
{
    private readonly IMongoCollection<AuditTrailEntity<T>> _collection;

    public AuditTrailsService(IOptions<AuditTrailDatabaseSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionStrings);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<AuditTrailEntity<T>>(settings.Value.ApplicationBetaCollectionName);
    }
    public async Task<List<AuditTrailEntity<T>>> GetAsync() => await _collection.Find(_ => true).ToListAsync();

    public async Task<AuditTrailEntity<T>?> GetAsync(string id) => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(AuditTrailEntity<T> entity) =>
        await _collection.InsertOneAsync(entity);

    public async Task UpdateAsync(string id, AuditTrailEntity<T> updatedEntity) =>
        await _collection.ReplaceOneAsync(x => x.Id == id, updatedEntity);

    public async Task RemoveAsync(string id) =>
        await _collection.DeleteOneAsync(x => x.Id == id);
    
}
