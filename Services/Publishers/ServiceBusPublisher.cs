using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace Services.Publishers
{
    public class ServiceBusPublisher
    {
        private readonly IConfiguration _configuration;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;

        public ServiceBusPublisher(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new ServiceBusClient(_configuration["ServiceBus:ConnectionString"]);
            _sender = _client.CreateSender(_configuration["ServiceBus:QueueName"]);
        }

        public async Task SendMessageAsync(string message)
        {
            ServiceBusMessage busMessage = new(message);
            await _sender.SendMessageAsync(busMessage);
            Console.WriteLine($"Mensagem enviada: {message}");
        }
    }
}
