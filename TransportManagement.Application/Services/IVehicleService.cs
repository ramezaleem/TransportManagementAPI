using TransportManagement.Domain.Entities;

namespace TransportManagement.Application.Services
{
    public interface IVehicleService
    {
        Task<(int TotalItems, IEnumerable<Vehicle> Items)> GetAllPaginatedAsync ( int pageNumber, int pageSize );
        Task<Vehicle> GetByIdAsync ( int id );
        Task<Vehicle> AddAsync ( Vehicle vehicle );
        Task<Vehicle> UpdateAsync ( Vehicle vehicle );
        Task<bool> DeleteAsync ( int id );
    }
}
