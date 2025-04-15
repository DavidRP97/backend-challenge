using System.Text.Json.Serialization;

namespace Domain.Records
{
    public record UpdateCnhImage
    ([property: JsonPropertyName("imagem_cnh")] string CnhImage);
}
