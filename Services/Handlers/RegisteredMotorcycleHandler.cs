using Data.Enums;
using Data.Mongo;
using Domain.Entities;
using MediatR;
using Serilog;
using Services.Events;
using Services.Publishers;
using System.Text.Json;

namespace Services.Handlers
{
    public class RegisteredMotorcycleHandler(IMongoConnection mongoConnection, ILogger logger, ServiceBusPublisher serviceBusPublisher) : INotificationHandler<RegisteredMotorcycleEvent>
    {
        private readonly ILogger _logger = logger;
        private readonly IMongoConnection _mongoConnection = mongoConnection;
        private readonly ServiceBusPublisher _serviceBusPublisher = serviceBusPublisher;
        public async Task Handle(RegisteredMotorcycleEvent notification, CancellationToken cancellationToken)
        {
            var motorcyle = await _mongoConnection.GetDocumentAsync<Motorcycle>(MongoCollections.Motorcyles, notification.Id.ToString());

            if (motorcyle.Year.Equals(2024))
            {
                _logger.Information($"O ano da moto placa: {motorcyle.Plate} é 2024!");
                await _serviceBusPublisher.SendMessageAsync(JsonSerializer.Serialize(motorcyle));
            }
        }
    }
}
