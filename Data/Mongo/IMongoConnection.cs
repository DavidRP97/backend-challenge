using MongoDB.Bson;

namespace Data.Mongo
{
    public interface IMongoConnection
    {
        Task<ObjectId> SetDocumentAsync<T>(T document, string collectionName);
        Task UpdateDocumentAsync<T>(T document, string collectionName, string id);
        Task<List<T>> GetDocumentsAsync<T>(string collectionName);
        Task<T> GetDocumentAsync<T>(string collectionName, string id);
        Task<List<T>> GetDocumentByFilterAsync<T>(string collectionName, string filterValue, string filterBy);
        Task DeleteDocumentAsync<T>(string collectionName, string id);
        Task<List<T>> GetWithTwoFiltersAsync<T>(string collectionName, string cnhNumber, string cnpj);
    }
}
