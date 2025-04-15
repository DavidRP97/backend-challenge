using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class MotorcycleRent : BaseMongoId
    {
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = "";
        [JsonPropertyName("entregador_id")]
        public string DeliveryMenId { get; set; } = "";
        [JsonPropertyName("moto_id")]
        public string MotorcycleId { get; set; } = "";
        [JsonPropertyName("data_inicio")]
        public DateOnly DateFrom { get; set; }
        [JsonPropertyName("data_termino")]
        public DateOnly DateTo { get; set; }
        [JsonPropertyName("data_previsao_termino")]
        public DateOnly EndExpectedDate { get; set; }
        [JsonPropertyName("plano")]
        public int Plan { get; set; }
    }
}
