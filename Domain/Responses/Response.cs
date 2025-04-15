using System.Text.Json.Serialization;

namespace Domain.Responses
{
    public class Response
    {
        [JsonPropertyName("mensagem")]
        public string Message { get; set; } = "";
        [JsonIgnore]
        public bool Success { get; set; }
        [JsonIgnore]
        public int StatusCode { get; set; }
    }
    public class Response<T> : Response
    {
        public T Data { get; set; }
    }

    public static class CustomResponses
    {
        public static Response Ok(string message) => new() { Message = message, Success = true, StatusCode = 200 };
        public static Response Created(string message) => new() { Message = message, Success = true, StatusCode = 201 };
        public static Response BadRequest(string message = "Dados inválidos") => new() { Message = message, Success = false, StatusCode = 400 };
        public static Response InternalServerError(string message = "Erro interno do servidor") => new() { Message = message, Success = false, StatusCode = 500 };

        public static Response<T> Ok<T>(T data, string message) => new() { Message = message, Success = true, StatusCode = 200, Data = data };
        public static Response<T> BadRequest<T>(string message = "Dados inválidos") => new() { Message = message, Success = false, StatusCode = 400 };
        public static Response<T> InternalServerError<T>(string message = "Erro interno do servidor") => new() { Message = message, Success = false, StatusCode = 500 };
    }
}
