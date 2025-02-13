using GettingStartedMassTransit.Consumer.Web.Models;
using GettingStartedMassTransit.Consumer.Web.Models.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GettingStartedMassTransit.Consumer.Web.Services
{
    public class MessageEventsService
    {
        private readonly IMongoCollection<AuditTrailEntity> _collection;

        public MessageEventsService(IOptions<EventStoreDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionStrings);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<AuditTrailEntity>(settings.Value.MessageEventCollectionName);
        }
        public async Task<List<AuditTrailEntity>> GetAsync() => await _collection.Find(_ => true).ToListAsync();

        public async Task<AuditTrailEntity?> GetAsync(string id) => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(AuditTrailEntity entity) =>
            await _collection.InsertOneAsync(entity);

        public async Task UpdateAsync(string id, AuditTrailEntity updatedEntity) =>
            await _collection.ReplaceOneAsync(x => x.Id == id, updatedEntity);

        public async Task RemoveAsync(string id) =>
            await _collection.DeleteOneAsync(x => x.Id == id);
    }
}
