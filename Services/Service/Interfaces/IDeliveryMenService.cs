using Domain.Entities;
using Domain.Records;
using Domain.Responses;

namespace Services.Service.Interfaces
{
    public interface IDeliveryMenService
    {
        Task<Response> InsertNewAsync(DeliveryMen model);
        Task<Response> UpdateCnhImageAsync(string id, UpdateCnhImage model);
    }
}
