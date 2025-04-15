using Domain.Entities;
using Domain.Records;
using Domain.Responses;

namespace Services.Service.Interfaces
{
    public interface IMotorcycleService
    {
        Task<Response> InsertAsync(Motorcycle model);
        Task<Response<IEnumerable<Motorcycle>>> GetAllAsync(string? plate);
        Task<Response<Motorcycle>> GetByIdAsync(string id);
        Task<Response> UpdatePlateAsync(UpdateMotorcyclePlate model, string id);
        Task<Response> DeleteAsync(string id);
        Task<Response> SaveYearNotificationAsync(Motorcycle model);
    }
}
