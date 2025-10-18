using TransportManagement.Domain.Entities;

namespace TransportManagement.Application.Services
{
    public interface ITransporterService
    {
        Task<(int TotalItems, IEnumerable<Transporter> Items)> GetAllPaginatedAsync ( int pageNumber, int pageSize );
        Task<Transporter> GetByIdAsync ( int id );
        Task<Transporter> AddAsync ( Transporter transporter );
        Task<Transporter> UpdateAsync ( Transporter transporter );
        Task<bool> DeleteAsync ( int id );
    }
}
