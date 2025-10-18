using TransportManagement.Domain.Entities;

namespace TransportManagement.Application.Services
{
    public interface ITripService
    {
        Task<(int TotalItems, IEnumerable<Trip> Items)> GetAllPaginatedAsync ( int pageNumber, int pageSize );
        Task<Trip> GetByIdAsync ( int id );
        Task<Trip> AddAsync ( Trip trip );
        Task<Trip> UpdateAsync ( Trip trip );
        Task<bool> DeleteAsync ( int id );
    }
}
