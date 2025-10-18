using TransportManagement.Domain.Entities;

namespace TransportManagement.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync ();
        Task<User> GetByIdAsync ( int id );
        Task<User> AddAsync ( User user );
        Task<User> UpdateAsync ( User user );
        Task<bool> DeleteAsync ( int id );


    }
}
