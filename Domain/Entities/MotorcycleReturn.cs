using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class MotorcycleReturn : BaseMongoId
    {
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = "";
        [JsonPropertyName("data_devolucao")]
        public DateOnly ReturnDate { get; set; }
        [JsonPropertyName("valor_diaria")]
        public double DailyValue { get; set; }
        [JsonPropertyName("valor_total")]
        public double TotalValue { get; set; }
        [JsonPropertyName("diarias_adicionais")]
        public int AdditionalDailyRate { get; set; }
        [JsonPropertyName("valor_diarias_adicionais")]
        public double ValueAdditionalDailyRate { get; set; }
        [JsonPropertyName("multas")]
        public double Fines { get; set; }
        [JsonPropertyName("detalhamento")]
        public string Details { get; set; } = "";
        [JsonPropertyName("detalhes_locação")]
        public MotorcycleRent MotorcycleRent { get; set; } = new();

    }
}
