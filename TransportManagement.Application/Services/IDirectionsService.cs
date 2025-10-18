using TransportManagement.Domain.Entities;

namespace TransportManagement.Application.Services
{
    public interface IDirectionsService
    {
        Task<(int TotalItems, IEnumerable<Direction> Items)> GetAllPaginatedAsync ( int pageNumber, int pageSize );
        Task<Direction> GetByIdAsync ( int id );
        Task<Direction> AddAsync ( Direction direction );
        Task<Direction> UpdateAsync ( Direction direction );
        Task<bool> DeleteAsync ( int id );
    }
}
