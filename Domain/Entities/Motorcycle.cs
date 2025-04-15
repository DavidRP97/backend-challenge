using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Motorcycle : BaseMongoId
    {
        private string _plate = "";
        [JsonPropertyName("placa")]
        [MaxLength(9)]
        public string Plate
        {
            get { return _plate; }
            set { _plate = value.ToUpper(); } 
        }
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = "";
        [JsonPropertyName("modelo")]
        public string Model { get; set; } = "";
        [JsonPropertyName("ano")]
        public int Year { get; set; }
    }
}
