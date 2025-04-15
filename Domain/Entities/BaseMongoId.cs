using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class BaseMongoId
    {
        [JsonIgnore]
        public ObjectId Id { get; set; }
    }
}
