using Data.Mongo;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using Services.Events;
using Services.Handlers;
using Services.Publishers;
using Services.Service;
using Services.Service.Interfaces;
using WebApi.WorkerService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(setup =>
{
    setup.EnableAnnotations();
});

builder.Services.AddSerilog(lc => lc
.MinimumLevel.Debug()
.MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
.Enrich.FromLogContext()
.WriteTo.Console());

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

// Registrar o MongoClient no contêiner de DI
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped<IMotorcycleService, MotorcycleService>();

builder.Services.AddScoped<IMongoConnection, MongoConnection>();

builder.Services.AddScoped<IDeliveryMenService, DeliveryMenService>();

builder.Services.AddScoped<ILeaseService, LeaseService>();  

builder.Services.AddSingleton<ServiceBusPublisher>();

builder.Services.AddHostedService<ServiceBusWorker>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies([typeof(RegisteredMotorcycleEvent).Assembly, typeof(RegisteredMotorcycleHandler).Assembly]);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
