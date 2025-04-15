using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class DeliveryMen : BaseMongoId
    {
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = "";
        [JsonPropertyName("nome")]
        public string Name { get; set; } = "";
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; } = "";
        [JsonPropertyName("data_nascimento")]
        public DateOnly BirthDate { get; set; }
        [JsonPropertyName("numero_cnh")]
        public string CnhNumber { get; set; } = "";
        [JsonPropertyName("tipo_cnh")]
        public string CnhCategory { get; set; } = "";
        [BsonIgnore]
        [JsonPropertyName("imagem_cnh")]
        public string CnhImage { get; set; } = "";
    }
}
