using System.Text.Json.Serialization;

namespace Domain.Records
{
    public record UpdateMotorcyclePlate
    (
        [property: JsonPropertyName("placa")] string Plate
    );
}
