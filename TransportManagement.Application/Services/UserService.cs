using Microsoft.EntityFrameworkCore;
using TransportManagement.Domain.Entities;
using TransportManagement.Infrastructure.Data;

namespace TransportManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly TransportDbContext _context;

        public UserService ( TransportDbContext context )
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync ()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User> GetByIdAsync ( int id )
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> AddAsync ( User user )
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync ( User user )
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync ( int id )
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
