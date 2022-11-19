using Microsoft.Extensions.Options;
using MongoDB.Driver;
using FeeCollectorApplication.Settings;
using FeeCollectorApplication.Models;

namespace FeeCollectorApplication.Service
{
    public class FeeCollectorService
    {
        public readonly IMongoCollection<Category> _categoryCollection;
        public FeeCollectorService(
    IOptions<FeeCollectorDatabaseSettings> feeCollectorDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                feeCollectorDatabaseSettings.Value.ConnectionURI);

            var mongoDatabase = mongoClient.GetDatabase(
                feeCollectorDatabaseSettings.Value.DatabaseName);

            _categoryCollection = mongoDatabase.GetCollection<Category>(
                feeCollectorDatabaseSettings.Value.CollectionName);
        }
        public async Task<List<Category>> GetAsync() =>
      await _categoryCollection.Find(_ => true).ToListAsync();
        public List<Category> GetData() => _categoryCollection.Find(_ => true).ToList();
        public async Task<Category?> GetAsync(string id) =>
            await _categoryCollection.Find(x => x._id == id).FirstOrDefaultAsync();
        public Category Get(string id)
        {
            return _categoryCollection.Find(x => x._id == id).FirstOrDefault();
        }
        public async Task CreateAsync(Category newObj) =>
            await _categoryCollection.InsertOneAsync(newObj);
        public void Create(Category newObj)
        {
            _categoryCollection.InsertOne(newObj);
        }
        public async Task UpdateAsync(string id, Category newObjUpdated) =>
            await _categoryCollection.ReplaceOneAsync(x => x._id == id, newObjUpdated);

        public void Update(string id, Category newObjUpdated)
        {
            _categoryCollection.ReplaceOne(x => x._id == id, newObjUpdated);
        }
        public async Task RemoveAsync(string id) =>
            await _categoryCollection.DeleteOneAsync(x => x._id == id);
        public async Task Remove(string id) =>
            _categoryCollection.DeleteOne(x => x._id == id);
    }

}
