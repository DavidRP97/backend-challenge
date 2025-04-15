using Domain.Entities;
using Domain.Records;
using Domain.Responses;

namespace Services.Service.Interfaces
{
    public interface ILeaseService
    {
        Task<Response> RentMotorcycleAsync(MotorcycleRent model);
        Task<Response> MotorcycleSetReturnDateAsync(SetReturnDate model, string id);
        Task<Response<MotorcycleReturn>> GetLeaseByIdAsync(string id);  
    }
}
