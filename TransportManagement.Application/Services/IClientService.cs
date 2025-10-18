using TransportManagement.Domain.Entities;

namespace TransportManagement.Application.Services
{
    public interface IClientService
    {
        Task<(int TotalItems, IEnumerable<Client> Items)> GetAllPaginatedAsync ( int pageNumber, int pageSize );
        Task<Client> GetByIdAsync ( int id );
        Task<Client> AddAsync ( Client client );
        Task<Client> UpdateAsync ( Client client );
        Task<bool> DeleteAsync ( int id );
    }
}
