using System.Text.Json.Serialization;

namespace Domain.Records
{
    public record SetReturnDate
    (
        [property: JsonPropertyName("data_devolucao")] DateOnly ReturnDate
    );
}
