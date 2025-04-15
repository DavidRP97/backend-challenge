using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Data.Mongo
{
    public class MongoConnection(IMongoClient mongoClient, IOptions<MongoDbSettings> mongoDbSettings) : IMongoConnection
    {
        private readonly IMongoDatabase _database = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);

        public async Task<T> GetDocumentAsync<T>(string collectionName, string id)
        {
            var collection = _database.GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            return await collection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task<List<T>> GetDocumentByFilterAsync<T>(string collectionName, string filterValue, string filterBy)
        {
            var collection = _database.GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq(filterBy, filterValue);
            return await collection.Find(filter).ToListAsync();
        }
        public async Task<List<T>> GetDocumentsAsync<T>(string collectionName)
        {
            var collection = _database.GetCollection<T>(collectionName);
            return await collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<ObjectId> SetDocumentAsync<T>(T document, string collectionName)
        {
            var collection = _database.GetCollection<T>(collectionName);
            await collection.InsertOneAsync(document);

            var idProperty = typeof(T).GetProperty("Id") ?? typeof(T).GetProperty("_id");

            if (idProperty != null && idProperty.PropertyType == typeof(ObjectId))
            {
                return (ObjectId)idProperty.GetValue(document)!;
            }
            else
            {
                throw new InvalidOperationException("O tipo T deve ter uma propriedade Id ou _id do tipo ObjectId.");
            }
        }

        public async Task UpdateDocumentAsync<T>(T document, string collectionName, string id)
        {
            var collection = _database.GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            await collection.ReplaceOneAsync(filter, document);
        }
        public async Task DeleteDocumentAsync<T>(string collectionName, string id)
        {
            var collection = _database.GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            await collection.DeleteOneAsync(filter);
        }

        public async Task<List<T>> GetWithTwoFiltersAsync<T>(string collectionName, string cnhNumber, string cnpj)
        {
            var collection = _database.GetCollection<T>(collectionName);

            var filter1 = Builders<T>.Filter.Eq("CnhNumber", cnhNumber);

            var filter2 = Builders<T>.Filter.Eq("Cnpj", cnpj);

            var combinedFilter = Builders<T>.Filter.Or(filter1, filter2);

            return await collection.Find(combinedFilter).ToListAsync();
        }
    }
}
