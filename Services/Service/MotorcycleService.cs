using Data.Enums;
using Data.Mongo;
using Domain;
using Domain.Entities;
using Domain.Records;
using Domain.Responses;
using MediatR;
using Serilog;
using Services.Events;
using Services.Service.Interfaces;

namespace Services.Service
{
    public class MotorcycleService(IMongoConnection mongoConnection, ILogger logger, IMediator mediator) : IMotorcycleService
    {
        private readonly IMongoConnection _mongoConnection = mongoConnection;
        private readonly IMediator _mediator = mediator;
        private readonly ILogger _logger = logger;
        public async Task<Response> DeleteAsync(string id)
        {
            try
            {
                var docs = await _mongoConnection.GetDocumentByFilterAsync<Motorcycle>(MongoCollections.Motorcyles, id, "Identifier");

                var validate = ValidateInputs(docs);

                if (!validate.Success) return validate;

                var doc = docs?.FirstOrDefault() ?? new();

                await _mongoConnection.DeleteDocumentAsync<Motorcycle>(MongoCollections.Motorcyles, doc.Id.ToString());

                return CustomResponses.Ok("Succesfully deleted");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Format());
                return CustomResponses.InternalServerError();
            }
        }

        public async Task<Response<IEnumerable<Motorcycle>>> GetAllAsync(string? plate)
        {
            if (string.IsNullOrEmpty(plate))
            {
                var result = await _mongoConnection.GetDocumentsAsync<Motorcycle>(MongoCollections.Motorcyles);

                return CustomResponses.Ok<IEnumerable<Motorcycle>>(result, "");
            }
            else
            {
                var result = await _mongoConnection.GetDocumentByFilterAsync<Motorcycle>(MongoCollections.Motorcyles, plate, "Plate");

                return CustomResponses.Ok<IEnumerable<Motorcycle>>(result, "");
            }
        }

        public async Task<Response<Motorcycle>> GetByIdAsync(string id)
        {
            var result = await _mongoConnection.GetDocumentByFilterAsync<Motorcycle>(MongoCollections.Motorcyles, id, "Identifier");

            return CustomResponses.Ok(result?.FirstOrDefault() ?? new(), "");
        }

        public async Task<Response> InsertAsync(Motorcycle model)
        {
            var checkPlate = await _mongoConnection.GetDocumentByFilterAsync<Motorcycle>(MongoCollections.Motorcyles, model.Plate, "Plate");

            if (checkPlate != null && checkPlate.Count != 0) return CustomResponses.BadRequest("Motorcycle already registered");

            var id = await _mongoConnection.SetDocumentAsync(model, MongoCollections.Motorcyles);

            _ = _mediator.Publish(new RegisteredMotorcycleEvent(id));

            return CustomResponses.Created("Succesfully data saved");
        }

        public async Task<Response> UpdatePlateAsync(UpdateMotorcyclePlate model, string id)
        {
            var docs = await _mongoConnection.GetDocumentByFilterAsync<Motorcycle>(MongoCollections.Motorcyles, id, "Identifier");

            var validate = ValidateInputs(docs);

            if (!validate.Success) return validate;

            var doc = docs.FirstOrDefault() ?? new();

            doc.Plate = model.Plate;

            await _mongoConnection.UpdateDocumentAsync(doc, MongoCollections.Motorcyles, doc.Id.ToString());

            return CustomResponses.Ok("Succesfully updated");
        }
        public async Task<Response> SaveYearNotificationAsync(Motorcycle model)
        {
            await _mongoConnection.SetDocumentAsync(new MotorcycleNotification { Plate = model.Plate, Year = model.Year }, MongoCollections.MotorcyleNotification);

            return CustomResponses.Ok("");
        }

        private static Response ValidateInputs<T>(T t)
        {
            if (t == null) return CustomResponses.BadRequest($"{nameof(T)} was null");
            return CustomResponses.Ok("");
        }       
    }
}
