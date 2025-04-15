using Data.Enums;
using Data.Mongo;
using Domain;
using Domain.Entities;
using Domain.Records;
using Domain.Responses;
using Serilog;
using Services.Service.Interfaces;
using System.Drawing.Printing;

namespace Services.Service
{
    public class LeaseService(ILogger logger, IMongoConnection mongoConnection) : ILeaseService
    {
        private readonly IMongoConnection _mongoConnection = mongoConnection;
        private readonly ILogger _logger = logger;
        public async Task<Response> RentMotorcycleAsync(MotorcycleRent model)
        {
            try
            {
                var rents = await _mongoConnection.GetDocumentByFilterAsync<MotorcycleRent>(MongoCollections.Leases, model.MotorcycleId, "MotorcycleId");

                if (rents.Count != 0)
                {
                    var today = DateTime.Now.Date;

                    var rent = rents.FirstOrDefault() ?? new();

                    var isRented = rent.EndExpectedDate > DateOnly.FromDateTime(today);

                    if (isRented)
                        return CustomResponses.BadRequest("Moto informada já está locada!");
                }

                var deliveryMen = await _mongoConnection.GetDocumentByFilterAsync<DeliveryMen>(MongoCollections.DeliveryMen, model.DeliveryMenId, "Identifier");

                if (deliveryMen.Count == 0)
                    return CustomResponses.BadRequest();

                var deliveryMan = deliveryMen.FirstOrDefault() ?? new();

                if (!deliveryMan.CnhCategory.Contains('a', StringComparison.InvariantCultureIgnoreCase))
                    return CustomResponses.BadRequest("Categoria de habilitação não permitida");

                var startDate = DateOnly.FromDateTime(DateTime.Now.Date.AddDays(1));

                model.DateFrom = startDate;

                model.EndExpectedDate = startDate.AddDays(model.Plan);

                await _mongoConnection.SetDocumentAsync(model, MongoCollections.Leases);

                return CustomResponses.Ok("");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Format());
                return CustomResponses.InternalServerError();
            }
        }
        public async Task<Response> MotorcycleSetReturnDateAsync(SetReturnDate model, string id)
        {
            var leases = await _mongoConnection.GetDocumentByFilterAsync<MotorcycleRent>(MongoCollections.Leases, id, "Identifier");

            var lease = leases.FirstOrDefault() ?? new();

            var returnTotalAmount = CalculateTotalAmount(lease, model.ReturnDate);

            var returnDateExist = await _mongoConnection.GetDocumentByFilterAsync<MotorcycleReturn>(MongoCollections.MotorcycleReturn, id, "Identifier");

            if(returnDateExist.Count != 0)
            {
                var doc = returnDateExist.FirstOrDefault() ?? new();
                returnTotalAmount.Id = doc.Id;
                await _mongoConnection.UpdateDocumentAsync(returnTotalAmount, MongoCollections.MotorcycleReturn, doc.Id.ToString());
            }
            else
            {
                await _mongoConnection.SetDocumentAsync(returnTotalAmount, MongoCollections.MotorcycleReturn);
            }
            return CustomResponses.Ok("Data de devolução informada com sucesso");
        }
        public async Task<Response<MotorcycleReturn>> GetLeaseByIdAsync(string id)
        {
            var leaseReturn = await _mongoConnection.GetDocumentByFilterAsync<MotorcycleReturn>(MongoCollections.MotorcycleReturn, id, "Identifier");

            return CustomResponses.Ok<MotorcycleReturn>(leaseReturn.FirstOrDefault() ?? new(), "");
        }

        private Dictionary<int, double> FinesTax = new()
        {
            { 7, 0.2 },
            { 15, 0.4 }
        };

        private Dictionary<int, double> AvailablePlans = new()
        {
            { 7, 30.0 },
            { 15, 28.0 },
            { 30, 22.0 },
            { 45, 20.0 },
            { 50, 18.0 }
        };

        private MotorcycleReturn CalculateTotalAmount(MotorcycleRent model, DateOnly returnDate)
        {
            var isRentDateReturnDate = model.DateTo == returnDate;

            var dailyValue = AvailablePlans[model.Plan];
            var totalValue = model.Plan * dailyValue;

            if (isRentDateReturnDate)
            {
                return new MotorcycleReturn
                {
                    Identifier = model.Identifier,
                    MotorcycleRent = model,
                    ReturnDate = returnDate,
                    TotalValue = totalValue,
                    DailyValue = dailyValue
                };
            }

            var isOverReturnDate = model.DateTo < returnDate;

            if (isOverReturnDate)
            {
                var diference = returnDate.DayNumber - model.DateTo.DayNumber;

                var adictional = diference * 50.00;

                return new MotorcycleReturn
                {
                    Identifier = model.Identifier,
                    MotorcycleRent = model,
                    ReturnDate = returnDate,
                    Details = $"Diárias adicionais: {diference} no valor de R$ 50,00 cada diária",
                    DailyValue = dailyValue,
                    AdditionalDailyRate = diference,
                    ValueAdditionalDailyRate = adictional,
                    Fines = 0.0,
                    TotalValue = totalValue + adictional
                };
            }
            else
            {
                var diference = model.DateTo.DayNumber - returnDate.DayNumber;

                var notEffectedDailys = diference * dailyValue;

                var fineTax = FinesTax[model.Plan];

                var valueFine = notEffectedDailys * fineTax;

                return new MotorcycleReturn
                {
                    Identifier = model.Identifier,
                    MotorcycleRent = model,
                    ReturnDate = returnDate,
                    Details = $"Diárias não utilizadas: {diference}. Multa de: {fineTax * 100}%",
                    DailyValue = dailyValue,
                    AdditionalDailyRate = 0,
                    ValueAdditionalDailyRate = 0,
                    Fines = valueFine,
                    TotalValue = (totalValue - notEffectedDailys) + valueFine
                };
            }
        }

    }
}
