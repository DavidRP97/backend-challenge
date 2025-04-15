using Azure.Messaging.ServiceBus;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Services.Service.Interfaces;
using System.Text.Json;

namespace WebApi.WorkerService
{
    public class ServiceBusWorker : BackgroundService
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly Serilog.ILogger _logger;
        private ServiceBusProcessor _processor;

        public ServiceBusWorker(Serilog.ILogger logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            var client = new ServiceBusClient(_configuration["ServiceBus:ConnectionString"]);
            _processor = client.CreateProcessor(_configuration["ServiceBus:QueueName"], new ServiceBusProcessorOptions());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync();

            _logger.Information("Worker iniciado e escutando mensagens...");
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();

            _logger.Information($"Mensagem recebida: {body}");

            using var scope = _serviceProvider.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<IMotorcycleService>();

            var motorcycle = JsonSerializer.Deserialize<Motorcycle>(body) ?? new();

            await service.SaveYearNotificationAsync(motorcycle);

            await args.CompleteMessageAsync(args.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.Error(args.Exception, "Erro ao processar mensagem");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
